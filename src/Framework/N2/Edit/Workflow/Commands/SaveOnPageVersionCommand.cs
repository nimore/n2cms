using System;
using N2.Edit.Versioning;

namespace N2.Edit.Workflow.Commands
{
    public class SaveOnPageVersionCommand : CommandBase<CommandContext>
    {
        private readonly IVersionManager versionMaker;

        public SaveOnPageVersionCommand(IVersionManager versionMaker)
        {
            this.versionMaker = versionMaker;
        }

        public override void Process(CommandContext state)
        {
            var item = state.Content;
            var page = Find.ClosestPage(item);
            
            if (!item.IsPage)
            {
                var parentVersion = page.FindPartVersion(item.Parent);

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
                else if (!parentVersion.Children.Contains(item))
                {
                    item.AddTo(parentVersion);
                    Utility.UpdateSortOrder(parentVersion.Children);
                }
            }

            if (page.VersionOf.HasValue)
            {
                versionMaker.UpdateVersion(page);
            }
            else
            {
                versionMaker.AddVersion(page, asPreviousVersion: false);
            }
        }
    }
}
