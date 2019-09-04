using System;
using N2.Edit.Versioning;
using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class SaveOnPageVersionCommand : CommandBase<CommandContext>
    {
        private readonly IContentItemRepository itemRepository;
        private readonly IVersionManager versionMaker;

        public SaveOnPageVersionCommand(IVersionManager versionMaker, IContentItemRepository itemRepository)
        {
            if (versionMaker == null)
            {
                throw new ArgumentNullException(nameof(versionMaker));
            }

            this.versionMaker = versionMaker;
            this.itemRepository = itemRepository;
        }

        public override void Process(CommandContext state)
        {
            var item = state.Content;
            var page = Find.ClosestPage(item);
            var pageVersion = page.VersionOf.HasValue
                ? page
                : versionMaker.AddVersion(page, asPreviousVersion: false);

            if (!item.IsPage)
            {
                var parentVersion = pageVersion.FindPartVersion(item.Parent);

                if (state.Parameters.ContainsKey("MoveBeforeVersionKey") && !string.IsNullOrEmpty(state.Parameters["MoveBeforeVersionKey"] as string))
                {
                    var beforeKey = (string)state.Parameters["MoveBeforeVersionKey"];
                    var beforeItem = page.FindDescendantByVersionKey(beforeKey);
                    beforeItem.Parent.InsertChildBefore(item, beforeItem.SortOrder);
                }
                else if (state.Parameters.ContainsKey("MoveBeforeSortOrder") && !string.IsNullOrEmpty(state.Parameters["MoveBeforeSortOrder"] as string))
                {
                    int beforeSortOrder = Convert.ToInt32(state.Parameters["MoveBeforeSortOrder"]);
                    parentVersion.InsertChildBefore(item, beforeSortOrder);
                }
                else
                {
                    var itemVersion = pageVersion.FindPartVersion(item);
                    if (!parentVersion.Children.Contains(itemVersion))
                    {
                        item.AddTo(parentVersion);
                        Utility.UpdateSortOrder(parentVersion.Children);
                    }
                }
                ////else if (item.ID == 0 && !parentVersion.Children.Contains(item))
                ////{
                ////    item.AddTo(parentVersion);
                ////    Utility.UpdateSortOrder(parentVersion.Children);
                ////}                
            }

            versionMaker.UpdateVersion(pageVersion);
        }        
    }
}
