using GitHubActionsDataCollector.Entities;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public class JobProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IJobProcessorMatchingService _jobProcessorMatchingService;

        public JobProcessorFactory(IServiceProvider serviceProvider,
                                    IJobProcessorMatchingService jobProcessorMatchingService)
        {
            _serviceProvider = serviceProvider;
            _jobProcessorMatchingService = jobProcessorMatchingService;
        }

        public IJobProcessor Create(WorkflowRunJob job, WorkflowRunSettings runSettings)
        {
            var processingType = _jobProcessorMatchingService.GetMatchingJobProcessor(job.Name, runSettings);

            if (processingType == null) return null;

            return (IJobProcessor)_serviceProvider.GetService(processingType);
        }
    }
}
