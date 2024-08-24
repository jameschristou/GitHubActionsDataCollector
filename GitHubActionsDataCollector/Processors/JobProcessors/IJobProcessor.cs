using GitHubActionsDataCollector.GitHubActionsApi;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public interface IJobProcessor
    {
        public Task Process(string owner, string repo, string token, WorkflowRunJobDto job, WorkflowRunArtifactsDto artifacts);
        public bool CanProcessJob(WorkflowRunJobDto job);
    }
}
