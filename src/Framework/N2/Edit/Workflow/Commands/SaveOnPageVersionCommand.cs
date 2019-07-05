using System;
using N2.Edit.Versioning;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class SaveOnPageVersionCommand : CommandBase<CommandContext>
    {
        private readonly IVersionManager versionMaker;
        private readonly IContentItemRepository itemRepository;

        public SaveOnPageVersionCommand(IVersionManager versionMaker, IContentItemRepository itemRepository)
        {
            this.versionMaker = versionMaker;
            this.itemRepository = itemRepository;
        }

        public override void Process(CommandContext state)
        {
            var item = state.Content;
            var page = Find.ClosestPage(item);
            ContentItem pageVersion;

            if(page.VersionOf.HasValue)
            {
                pageVersion = page;
                if (!item.IsPage)
                {
                    var parentVersion = pageVersion.FindPartVersion(item.Parent);

                    if (state.Parameters.ContainsKey("MoveBeforeVersionKey") && !string.IsNullOrEmpty(state.Parameters["MoveBeforeVersionKey"] as string))
                    {
                        var beforeKey = (string)state.Parameters["MoveBeforeVersionKey"];
                        var beforeItem = pageVersion.FindDescendantByVersionKey(beforeKey);
                        beforeItem.Parent.InsertChildBefore(item, beforeItem.SortOrder);
                    }
                    else if (state.Parameters.ContainsKey("MoveBeforeSortOrder") && !string.IsNullOrEmpty(state.Parameters["MoveBeforeSortOrder"] as string))
                    {
                        int beforeSortOrder = Convert.ToInt32(state.Parameters["MoveBeforeSortOrder"]);
                        parentVersion.InsertChildBefore(item, beforeSortOrder);
                    }
                    else
                    {
                        item.AddTo(parentVersion);
                        Utility.UpdateSortOrder(parentVersion.Children);
                    }
                }

                versionMaker.UpdateVersion(pageVersion);
            }
            else
            {
                pageVersion = versionMaker.AddVersion(page, asPreviousVersion: false);
                ////itemRepository.SaveOrUpdate(pageVersion);
            }            
        }
    }
}
