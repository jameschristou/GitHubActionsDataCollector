using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Processors.JobProcessors;

namespace GitHubActionsDataCollector.Processors
{
    public interface IWorkflowRunJobProcessor
    {
        public Task Process(WorkflowRunJobDto job, WorkflowRunArtifactsDto artifacts);
    }

    public class WorkflowRunJobProcessor : IWorkflowRunJobProcessor
    {
        private readonly IJobProcessor _jobProcessor;

        public WorkflowRunJobProcessor(IJobProcessor jobProcessor)
        {
            _jobProcessor = jobProcessor;
        }

        public async Task Process(WorkflowRunJobDto job, WorkflowRunArtifactsDto artifacts)
        {
            Console.WriteLine($"Processing job:{job.id} in run:{job.run_id} attempt:{job.run_attempt}");

            // check if this job is registered for any special processing. This will be configuration based per workflow. There will be a set number of built in special processors
            // but you can also implement your own custom processor
            // first thing to do is check which JobProcessors need to be run for this job

            // right now we only have one processor but if we end up with multiple then we might need a factory approach here
            if (_jobProcessor.CanProcessJob(job))
            {
                await _jobProcessor.Process(job, artifacts);
            }
        }
    }
}
