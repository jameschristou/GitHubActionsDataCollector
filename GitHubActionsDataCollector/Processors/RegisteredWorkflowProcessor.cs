using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApiClient;

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
        private const int MaxBatchSize = 3;
        private const int ResultsPerPage = 1;

        public RegisteredWorkflowProcessor(IWorkflowRunProcessor workflowRunProcessor, IGitHubActionsApiClient gitHibActionsApiClient)
        {
            _workflowRunProcessor = workflowRunProcessor;
            _gitHibActionsApiClient = gitHibActionsApiClient;
        }

        public async Task Process(RegisteredWorkflow registeredWorkflow)
        {
            var pageNumber = 0;
            int numProcessed = 0;
            int totalResults;

            var fromDate = registeredWorkflow.LastProcessedWorkflowRun != null ? registeredWorkflow.LastProcessedWorkflowRun.CompletedAtUtc : DateTime.MinValue;

            do
            {
                pageNumber++;

                var workflowRuns = await _gitHibActionsApiClient.GetWorkflowRuns(registeredWorkflow.Owner, registeredWorkflow.Repo, registeredWorkflow.WorkflowId, fromDate, pageNumber, ResultsPerPage);
                totalResults = workflowRuns.total_count;

                // then process each workflow run returned
                foreach (var workflowRun in workflowRuns.workflow_runs)
                {
                    await _workflowRunProcessor.Process(registeredWorkflow, workflowRun);
                }

                numProcessed++;
            }
            while (numProcessed < MaxBatchSize && pageNumber * ResultsPerPage < totalResults);
        }
    }
}
