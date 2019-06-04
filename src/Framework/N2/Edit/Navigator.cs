using System;
using N2.Engine;
using N2.Persistence;
using N2.Persistence.Sources;
using N2.Web;

namespace N2.Edit
{
    [Service]
    public class Navigator
    {
        private readonly IHost host;
        private readonly IPersister persister;
        private readonly ContentSource sources;
        private readonly VirtualNodeFactory virtualNodes;

        public Navigator(IPersister persister, IHost host, VirtualNodeFactory nodes, ContentSource sources)
        {
            this.persister = persister;
            this.host = host;
            this.virtualNodes = nodes;
            this.sources = sources;
        }

        public virtual ContentItem Navigate(ContentItem startingPoint, string path)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("Navigate: {0}; {0}", startingPoint.Url, path));

            return startingPoint.GetChild(path)
                ?? sources.ResolvePath(startingPoint, path).CurrentItem
                ?? virtualNodes.Get(startingPoint.Path + path.TrimStart('/'))
                ?? virtualNodes.Get(path);
        }

        public virtual ContentItem Navigate(string path)
        {
            ////TraceSources.AzureTraceSource.TraceInformation(string.Format("Navigate: {0}", path));

            if (path == null)
                return null;

            if (!path.StartsWith("/"))
            {
                if (path.StartsWith("~"))
                {
                    return Navigate(persister.Get(host.CurrentSite.StartPageID), path.Substring(1))
                        ?? sources.ResolvePath(path).CurrentItem
                        ?? virtualNodes.Get(path);
                }
                throw new ArgumentException("The path must start with a slash '/', was '" + path + "'", "path");
            }

            return Navigate(persister.Get(host.CurrentSite.RootItemID), path);
        }
    }
}
