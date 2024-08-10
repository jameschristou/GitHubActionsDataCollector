using FluentNHibernate.Mapping;
using GitHubActionsDataCollector.Entities;

namespace GitHubActionsDataCollector.Mappings
{
    public class RegisteredWorkflowMap : ClassMap<RegisteredWorkflow>
    {
        public RegisteredWorkflowMap()
        {
            Id(x => x.Id);
            Map(x => x.Owner);
            Map(x => x.Repo);
            Map(x => x.WorkflowId);
            Map(x => x.WorkflowName);
            Map(x => x.Token);
            Map(x => x.LastCheckedAtUtc);
        }
    }
}
