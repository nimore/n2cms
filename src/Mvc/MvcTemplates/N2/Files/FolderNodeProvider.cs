using System;
using System.Collections.Generic;
using N2.Edit;
using N2.Edit.FileSystem;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Persistence;
using N2.Security;

namespace N2.Management.Files
{
    /// <summary>
    /// A custom <see cref="INodeProvider" /> that enumerates the FileSystem as special ContentItem instances
    /// </summary>
    [Service]
    public class FolderNodeProvider : INodeProvider
    {
        private IDependencyInjector dependencyInjector;
        private IFileSystem fs;
        private IRepository<ContentItem> repository;

        public FolderNodeProvider(IFileSystem fs, IRepository<ContentItem> repository, IDependencyInjector dependencyInjector)
        {
            UploadFolderPaths = new FolderReference[0];
            this.fs = fs;
            this.repository = repository;
            this.dependencyInjector = dependencyInjector;
        }

        internal FolderReference[] UploadFolderPaths { get; set; }

        #region INodeProvider Members

        public ContentItem Get(string path)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("Get: {0}", path));

            foreach (var pair in UploadFolderPaths)
            {
                if (path.StartsWith(pair.Path, StringComparison.InvariantCultureIgnoreCase))
                {
                    var dir = CreateDirectory(pair);

                    string remaining = path.Substring(pair.Path.Length);
                    if (string.IsNullOrEmpty(remaining))
                        return dir;
                    return dir.GetChild(remaining);
                }
            }

            return null;
        }

        public IEnumerable<ContentItem> GetChildren(string path)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("GetChildren: {0}", path));

            foreach (var pair in UploadFolderPaths)
            {
                if (pair.ParentPath.Equals(path, StringComparison.InvariantCultureIgnoreCase))
                {
                    yield return CreateDirectory(pair);
                }
                else if (path != null && path.StartsWith(pair.Path, StringComparison.InvariantCultureIgnoreCase))
                {
                    ContentItem dir = CreateDirectory(pair);

                    var subdirPath = path.Substring(pair.Path.Length);
                    if (subdirPath != "")
                    {
                        dir = dir.GetChild(subdirPath);
                    }

                    if (dir != null)
                    {
                        ////foreach (var child in dir.GetChildren(new NullFilter()))
                        foreach (var child in dir.GetChildPagesUnfiltered())
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        internal static Directory CreateDirectory(FileSystemRoot folder, IFileSystem fs, IRepository<ContentItem> persister, IDependencyInjector dependencyInjector)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("CreateDirectory: {0}", folder.Path));

            var dd = fs.GetDirectoryOrVirtual(folder.Path);
            var parent = persister.Get(folder.GetParentID());

            var dir = Directory.New(dd, parent, dependencyInjector);
            dir.Name = folder.GetName();
            dir.Title = folder.Title ?? dir.Name;
            dir.UrlPrefix = folder.UrlPrefix;

            Apply(folder.Readers, dir);
            Apply(folder.Writers, dir);

            return dir;
        }

        private static void Apply(PermissionMap map, Directory dir)
        {
            if (map.IsAltered)
                DynamicPermissionMap.SetRoles(dir, map.Permissions, map.Roles);
            else
                DynamicPermissionMap.SetAllRoles(dir, map.Permissions);
        }

        private Directory CreateDirectory(FolderReference pair)
        {
            return CreateDirectory(pair.Folder, fs, repository, dependencyInjector);
        }

        #endregion INodeProvider Members
    }
}
