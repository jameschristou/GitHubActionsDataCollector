using FluentNHibernate.Mapping;
using GitHubActionsDataCollector.Entities;

namespace GitHubActionsDataCollector.Mappings
{
    public class WorkflowRunJobMap : ClassMap<WorkflowRunJob>
    {
        public WorkflowRunJobMap()
        {
            Id(x => x.Id);
            Map(x => x.RunId);
            Map(x => x.JobId);
            Map(x => x.RunAttempt);
            Map(x => x.Name);
            Map(x => x.Conclusion);
            Map(x => x.StartedAtUtc);
            Map(x => x.CompletedAtUtc);
            Map(x => x.Url);
            References(x => x.WorkflowRun);
        }
    }
}
