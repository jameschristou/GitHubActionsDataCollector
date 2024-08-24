using FluentNHibernate.Mapping;
using GitHubActionsDataCollector.Entities;

namespace GitHubActionsDataCollector.Mappings
{
    public class TestResultMap : ClassMap<TestResult>
    {
        public TestResultMap() 
        {
            Id(x => x.Id);
            Map(x => x.Name);
            Map(x => x.Result);
            Map(x => x.DurationMs);
            References(x => x.WorkflowRunJob, "WorkflowRunJobId");
        }
    }
}
