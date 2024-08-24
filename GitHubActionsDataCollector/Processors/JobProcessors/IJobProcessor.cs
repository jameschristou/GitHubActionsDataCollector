using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public interface IJobProcessor
    {
        public Task Process(string owner, string repo, string token, WorkflowRunJob job, WorkflowRunArtifactsDto artifacts);
        public bool CanProcessJob(WorkflowRunJob job);
    }
}
