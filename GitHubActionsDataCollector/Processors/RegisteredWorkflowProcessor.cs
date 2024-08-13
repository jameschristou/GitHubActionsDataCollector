using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Repositories;

namespace GitHubActionsDataCollector.Processors
{
    public interface IRegisteredWorkflowProcessor
    {
        public Task Process(RegisteredWorkflow registeredWorkflow);
    }

    public class RegisteredWorkflowProcessor : IRegisteredWorkflowProcessor
    {
        private readonly IWorkflowRunProcessor _workflowRunProcessor;
        private readonly IGitHubActionsApiClient _gitHibActionsApiClient;
        private readonly IRegisteredWorkflowRepository _registeredWorkflowRepository;
        private const int MaxBatchSize = 3;
        private const int ResultsPerPage = 4;
        private const int SearchWindowInHours = 24;

        public RegisteredWorkflowProcessor(IWorkflowRunProcessor workflowRunProcessor, 
                                            IGitHubActionsApiClient gitHibActionsApiClient,
                                            IRegisteredWorkflowRepository registeredWorkflowRepository)
        {
            _workflowRunProcessor = workflowRunProcessor;
            _gitHibActionsApiClient = gitHibActionsApiClient;
            _registeredWorkflowRepository = registeredWorkflowRepository;
        }

        public async Task Process(RegisteredWorkflow registeredWorkflow)
        {
            var pageNumber = 0;
            int totalResults;

            var fromDate = registeredWorkflow.ProcessedUntilUtc;
            var toDate = fromDate.AddHours(SearchWindowInHours);
            var processedUntilDate = toDate;

            do
            {
                pageNumber++;

                var workflowRuns = await _gitHibActionsApiClient.GetWorkflowRuns(registeredWorkflow.Owner, registeredWorkflow.Repo, registeredWorkflow.Token, registeredWorkflow.WorkflowId, fromDate, toDate, pageNumber, ResultsPerPage);
                totalResults = workflowRuns.total_count;

                // then process each workflow run returned
                foreach (var workflowRun in workflowRuns.workflow_runs)
                {
                    await _workflowRunProcessor.Process(registeredWorkflow, workflowRun);

                    processedUntilDate = DateTime.Parse(workflowRun.created_at);
                }
            }
            while (pageNumber < MaxBatchSize && pageNumber * ResultsPerPage < totalResults);

            // now we need to update the ProcessedUntilUtc
            registeredWorkflow.ProcessedUntilUtc = processedUntilDate;
            registeredWorkflow.LastCheckedAtUtc = DateTime.UtcNow;

            await _registeredWorkflowRepository.Update(registeredWorkflow);
        }
    }
}
