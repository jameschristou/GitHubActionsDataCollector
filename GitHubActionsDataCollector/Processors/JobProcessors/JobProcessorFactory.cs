using GitHubActionsDataCollector.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public class JobProcessorFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public JobProcessorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJobProcessor Create(WorkflowRunJob job)
        {
            if (DotNetXmlTestResultsProcessor.CanProcessJob(job))
            {
                return GetService<DotNetXmlTestResultsProcessor>();
            }

            if (CypressTestResultsProcessor.CanProcessJob(job))
            {
                return GetService<CypressTestResultsProcessor>();
            }

            return null;
        }

        private T GetService<T>()
        {
            // need to make sure we resolve using the current scope because some of the services 
            return _serviceProvider.GetService<T>();
        }
    }
}
