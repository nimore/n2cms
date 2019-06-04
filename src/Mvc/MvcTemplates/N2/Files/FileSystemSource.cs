using System;
using System.Collections.Generic;
using System.Linq;
using N2.Definitions;
using N2.Edit;
using N2.Edit.FileSystem.Items;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
using N2.Web;

namespace N2.Management.Files
{
    [Service(typeof(SourceBase))]
    public class FileSystemSource : SourceBase<IFileSystemNode>
    {
        private FolderNodeProvider nodes;

        public FileSystemSource(FolderNodeProvider nodes)
        {
            this.nodes = nodes;
        }

        public override IEnumerable<ContentItem> AppendChildren(IEnumerable<ContentItem> previousChildren, Query query)
        {
            return previousChildren;
        }

        public override ContentItem Copy(ContentItem source, ContentItem destination)
        {
            return Active(source).CopyTo(destination);
        }

        public override void Delete(ContentItem item)
        {
            Active(item).Delete();
        }

        public override IEnumerable<ContentItem> FilterChildren(IEnumerable<ContentItem> previousChildren, Query query)
        {
            if (query.Interface != Interfaces.Managing || (query.OnlyPages.HasValue && query.OnlyPages.Value != true))
                return previousChildren;

            return previousChildren.Union(nodes.GetChildren(query.Parent.Path));
        }

        public override ContentItem Get(object id)
        {
            var path = id as string;
            if (path == null) return null;

            return nodes.Get(path);
        }

        public override bool IsProvidedBy(N2.ContentItem item)
        {
            return base.IsProvidedBy(item) && !(item is RootDirectory);
        }

        public override ContentItem Move(ContentItem source, ContentItem destination)
        {
            Active(source).MoveTo(destination);
            return source;
        }

        public override PathData ResolvePath(ContentItem startingPoint, string path)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(String.Format("ResolvePath: {0}; {1}", startingPoint.Url, path));
            return ResolvePath(startingPoint.Path + path.TrimStart('/'));
        }

        public override PathData ResolvePath(string path)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(String.Format("ResolvePath: {0}", path));
            var item = nodes.Get(path);
            if (item == null)
                return PathData.Empty;

            return item.FindPath(PathData.DefaultAction);
        }

        public override void Save(ContentItem item)
        {
            Active(item).Save();
        }

        private IActiveContent Active(ContentItem item)
        {
            return (IActiveContent)item;
        }
    }
}
