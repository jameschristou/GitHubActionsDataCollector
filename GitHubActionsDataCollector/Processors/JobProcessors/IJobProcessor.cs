using GitHubActionsDataCollector.GitHubActionsApi;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public interface IJobProcessor
    {
        public Task Process(WorkflowRunJobDto job, WorkflowRunArtifactsDto artifacts);
        public bool CanProcessJob(WorkflowRunJobDto job);
    }
}
