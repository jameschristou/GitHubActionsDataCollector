using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApiClient;

namespace GitHubActionsDataCollector
{
    internal class WorkflowProcessor
    {
        private readonly IWorkflowRunProcessor _workflowRunProcessor;
        private readonly IGitHubActionsApiClient _gitHibActionsApiClient;

        public WorkflowProcessor(IWorkflowRunProcessor workflowRunProcessor, IGitHubActionsApiClient gitHibActionsApiClient)
        {
            _workflowRunProcessor = workflowRunProcessor;
            _gitHibActionsApiClient = gitHibActionsApiClient;
        }

        public async Task Process(RegisteredWorkflow registeredWorkflow)
        {
            // hardcode the workflowId for now
            var pageNumber = 0;
            var resultsPerPage = 1;
            var maxBatchSize = 2;
            int numProcessed = 0;
            int totalResults;

            do
            {
                pageNumber++;

                var workflowRuns = await _gitHibActionsApiClient.GetWorkflowRuns(registeredWorkflow.Owner, registeredWorkflow.Repo, registeredWorkflow.WorkflowId, DateTime.Now.AddDays(-2), pageNumber, resultsPerPage);
                totalResults = workflowRuns.total_count;

                // then process each workflow run returned
                foreach (var workflowRun in workflowRuns.workflow_runs)
                {
                    await _workflowRunProcessor.Process(registeredWorkflow.Owner, registeredWorkflow.Repo, workflowRun);
                }

                numProcessed++;
            }
            while (numProcessed < maxBatchSize && pageNumber * resultsPerPage < totalResults);
        }
    }
}
