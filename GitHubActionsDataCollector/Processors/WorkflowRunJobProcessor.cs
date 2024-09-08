using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Processors.JobProcessors;

namespace GitHubActionsDataCollector.Processors
{
    public interface IWorkflowRunJobProcessor
    {
        public Task Process(string repoOwner, string repoName, string token, WorkflowRunJob job, WorkflowRunArtifactsDto artifacts, WorkflowRunSettings runSettings);
    }

    public class WorkflowRunJobProcessor : IWorkflowRunJobProcessor
    {
        private readonly JobProcessorFactory _jobProcessorFactory;

        public WorkflowRunJobProcessor(JobProcessorFactory jobProcessorFactory)
        {
            _jobProcessorFactory = jobProcessorFactory;
        }

        public async Task Process(string repoOwner, string repoName, string token, WorkflowRunJob job, WorkflowRunArtifactsDto artifacts, WorkflowRunSettings runSettings)
        {
            Console.WriteLine($"Processing job:{job.JobId} in run:{job.RunId} attempt:{job.RunAttempt}");

            // check if this job is registered for any special processing. This will be configuration based per workflow. There will be a set number of built in special processors
            // but you can also implement your own custom processor
            // first thing to do is check which JobProcessors need to be run for this job

            var jobProcessor = _jobProcessorFactory.Create(job, runSettings);

            if (jobProcessor != null)
            {
                await jobProcessor.Process(repoOwner, repoName, token, job, artifacts);
            }
        }
    }
}
