using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using N2.Collections;
using N2.Definitions;
using N2.Security;
using N2.Web.Drawing;

namespace N2.Edit.FileSystem.Items
{
    public abstract class AbstractDirectory : AbstractNode, IFileSystemDirectory
    {
        private static readonly TitleComparer<Directory> directoryTitleComparer = new TitleComparer<Directory>();
        private static readonly TitleComparer<File> fileTitleComparer = new TitleComparer<File>();

        public string UrlPrefix { get; set; }

        public static AbstractDirectory EnsureDirectory(ContentItem item)
        {
            if (item is AbstractDirectory)
                return item as AbstractDirectory;

            throw new N2Exception(item + " is not a Directory.");
        }

        /// <summary>
        /// Gets the files and directories that are contained within this AbstractDirectory.
        /// </summary>
        /// <returns>ItemList of files and directories within this AbstractDirectory.</returns>
        public override ItemList GetChildPagesUnfiltered()
        {
            ////TraceSources.AzureTraceSource.TraceInformation("GetChildPagesUnfiltered");

            var items = new ItemList();
            items.AddRange(GetDirectories());
            items.AddRange(GetFiles());
            return items;
        }

        /// <summary>
        /// Gets the child parts that are part of this AbstractDirectory. This is always an empty ItemList.
        /// </summary>
        /// <param name="zoneName"></param>
        /// <returns>Always returns an empty ItemList.</returns>
        public override ItemList GetChildPartsUnfiltered(string zoneName = null)
        {
            ////TraceSources.AzureTraceSource.TraceInformation("GetChildPartsUnfiltered");

            return new ItemList();
        }

        [Obsolete("Use GetChildPagesUnfiltered to get the directories and files below this element.")]
        public override ItemList GetChildren(ItemFilter filter)
        {
            ////TraceSources.AzureTraceSource.TraceInformation("GetChildren");

            ItemList items = new ItemList();
            items.AddRange(filter.Pipe(GetDirectories()));
            items.AddRange(filter.Pipe(GetFiles()));
            return items;
        }

        public virtual IList<Directory> GetDirectories()
        {
            ////TraceSources.AzureTraceSource.TraceInformation("GetDirectories");

            try
            {
                List<Directory> directories = new List<Directory>();
                foreach (DirectoryData dir in FileSystem.GetDirectories(LocalUrl))
                {
                    var node = Items.Directory.New(dir, this, DependencyInjector);
                    node.UrlPrefix = this.UrlPrefix;
                    if (!DynamicPermissionMap.IsAllRoles(this, Permission.Read))
                        DynamicPermissionMap.SetRoles(node, Permission.Read, DynamicPermissionMap.GetRoles(this, Permission.Read).ToArray());
                    if (!DynamicPermissionMap.IsAllRoles(this, Permission.Write))
                        DynamicPermissionMap.SetRoles(node, Permission.Write, DynamicPermissionMap.GetRoles(this, Permission.Write).ToArray());
                    directories.Add(node);
                }
                directories.Sort(directoryTitleComparer);
                return directories;
            }
            catch (DirectoryNotFoundException ex)
            {
                Engine.Logger.Warn(ex);
                return new List<Directory>();
            }
        }

        public virtual IList<File> GetFiles()
        {
            ////TraceSources.AzureTraceSource.TraceInformation("GetFiles");

            try
            {
                List<File> files = new List<File>();

                var fileMap = new Dictionary<string, File>(StringComparer.OrdinalIgnoreCase);
                foreach (var fd in FileSystem.GetFiles(LocalUrl).OrderBy(fd => fd.Name))
                {
                    var file = new File(fd, this);
                    file.Set(FileSystem);
                    file.Set(ImageSizes);

                    var unresizedFileName = ImageSizes.RemoveImageSize(file.Name);
                    if (unresizedFileName != null && fileMap.ContainsKey(unresizedFileName))
                    {
                        fileMap[unresizedFileName].Add(file);

                        if (ImageSizes.GetSizeName(file.Name) == "icon")
                            file.IsIcon = true;
                    }
                    else
                    {
                        files.Add(file);
                        fileMap[file.Name] = file;
                    }
                }
                files.Sort(fileTitleComparer);
                return files;
            }
            catch (DirectoryNotFoundException ex)
            {
                Engine.Logger.Warn(ex);
                return new List<File>();
            }
        }

        protected override ContentItem FindNamedChild(string nameSegment)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("FindNamedChild: {0}", nameSegment));

            return GetFiles().FirstOrDefault(f => f.Name == nameSegment) ??
                   (ContentItem)GetDirectories().FirstOrDefault(d => d.Name == nameSegment);
        }

        private static string GetSizeName(string lastFileName, string lastFileExtension, File file)
        {
            int lastFileNameLength = (lastFileName + ImagesUtility.Separator).Length;
            string size = file.Name.Substring(lastFileNameLength, file.Name.Length - lastFileNameLength - lastFileExtension.Length);
            return size;
        }

        private class TitleComparer<T> : IComparer<T> where T : ContentItem
        {
            #region IComparer<ContentItem> Members

            public int Compare(T x, T y)
            {
                return StringComparer.InvariantCultureIgnoreCase.Compare(x.Title, y.Title);
            }

            #endregion IComparer<ContentItem> Members
        }
    }
}
