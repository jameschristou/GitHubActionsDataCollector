using FluentNHibernate.Mapping;
using GitHubActionsDataCollector.Entities;

namespace GitHubActionsDataCollector.Mappings
{
    public class WorkflowRunMap : ClassMap<WorkflowRun>
    {
        public WorkflowRunMap() 
        {
            Id(x => x.Id);
            Map(x => x.Owner);
            Map(x => x.Repo);
            Map(x => x.RunId);
            Map(x => x.WorkflowId);
            Map(x => x.WorkflowName);
            Map(x => x.Title);
            Map(x => x.Url);
            Map(x => x.StartedAtUtc);
            Map(x => x.CompletedAtUtc);
            Map(x => x.NumAttempts);
            Map(x => x.Conclusion);
        }
    }
}
