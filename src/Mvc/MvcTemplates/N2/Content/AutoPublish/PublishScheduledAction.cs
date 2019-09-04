using System;
using System.Diagnostics;
using System.Linq;
using N2.Persistence;
using N2.Persistence.Finder;
using N2.Plugin.Scheduling;
using N2.Security;
using N2.Edit.Workflow;
using N2.Edit.Versioning;
using N2.Engine;
using N2.Web;

namespace N2.Edit.AutoPublish
{
    [Service]
    [ScheduleExecution(30, TimeUnit.Seconds)]
    public class PublishScheduledAction : ScheduledAction
    {
        private readonly ContentVersionRepository versionRepository;
        private readonly IVersionManager versioner;
        private readonly IPersister persister;
        private readonly ISecurityManager security;
        private readonly StateChanger changer;
        private readonly IErrorNotifier errorNotifier;

        public PublishScheduledAction(ContentVersionRepository versionRepository, IVersionManager versioner, IPersister persister, ISecurityManager security, StateChanger changer, IErrorNotifier errorNotifier)
        {
            this.versionRepository = versionRepository;
            this.versioner = versioner;
            this.persister = persister;
            this.security = security;
            this.changer = changer;
            this.errorNotifier = errorNotifier;
        }

        public virtual void MarkForFuturePublishing(ContentItem item, DateTime futureDate)
        {
            VersioningExtensions.MarkForFuturePublishing(changer, item, futureDate);
        }

        public override void Execute()
        {
            using (security.Disable())
            {
                PublishPendingVersions();

                ChangeStateToItemsBecomePublished();

                ChangeStateToExpiredItems();
            }
        }

        private void ChangeStateToExpiredItems()
        {
            var implicitExpire = persister.Repository
                .Find(Parameter.LessOrEqual("Expires", Utility.CurrentTime())
                    & Parameter.Equal("State", ContentState.Published))
                .ToList();
            for (int i = 0; i < implicitExpire.Count; i++)
            {
                try
                {
                    // reset status on expired items
                    var item = implicitExpire[i];
                    changer.ChangeTo(item, ContentState.Unpublished);
                    persister.Save(item);
                }
                catch(Exception e)
                {
                    errorNotifier.Notify(e);
                }
            }
        }

        private void ChangeStateToItemsBecomePublished()
        {
            var implicitAutoPublish = persister.Repository
                .Find(Parameter.LessThan("Published", Utility.CurrentTime())
                    & Parameter.Equal("State", ContentState.Waiting))
                .ToList();
            for (int i = 0; i < implicitAutoPublish.Count; i++)
            {
                try
                {
                    // saving the master version for auto-publish will be eventually become published without this, but we want to update the state
                    var item = implicitAutoPublish[i];
                    changer.ChangeTo(item, ContentState.Published);
                    persister.Save(item);
                }
                catch(Exception e)
                {
                    errorNotifier.Notify(e);
                }
            }
        }

        private void PublishPendingVersions()
        {
            var scheduledForAutoPublish = versionRepository.GetVersionsScheduledForPublish(Utility.CurrentTime()).ToList();
            foreach (var version in scheduledForAutoPublish)
            {
                try
                {
                    var scheduledVersion = versionRepository.DeserializeVersion(version);
                    scheduledVersion["FuturePublishDate"] = null;
                    versioner.Publish(persister, scheduledVersion);
                }
                catch(Exception e)
                {
                    errorNotifier.Notify(e);
                }
            }
          }
    }
}
