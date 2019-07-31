using N2.Persistence;

namespace N2.Edit.Workflow.Commands
{
    public class SaveCommand : CommandBase<CommandContext>
    {
        IPersister persister;
        public SaveCommand(IPersister persister)
        {
            this.persister = persister;
        }

        public override void Process(CommandContext state)
        {
            persister.Save(state.Content);
            foreach (ContentItem item in state.GetItemsToSave())
            {
                if (item != state.Content)
                {
                    if(item.Parent != null && item.Parent.ID == 0 && item.Parent.VersionOf.HasValue && item.Parent.VersionOf.ID == state.Content.ID)
                    {
                        item.AddTo(state.Content);
                    }
                    persister.Save(item);
					SaveChildren(item);
                }
            }
        }

		private void SaveChildren(ContentItem parent)
		{
			foreach (var child in parent.Children)
			{
				persister.Save(child);
				SaveChildren(child);
			}
		}
    }
}
