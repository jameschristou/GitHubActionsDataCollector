using GitHubActionsDataCollector.GitHubActionsApiClient;

namespace GitHubActionsDataCollector
{
    internal class Processor
    {
        private readonly IWorkflowRunProcessor _workflowRunProcessor;
        private readonly IGitHubActionsApiClient _gitHibActionsApiClient;

        public Processor(IWorkflowRunProcessor workflowRunProcessor, IGitHubActionsApiClient gitHibActionsApiClient)
        {
            _workflowRunProcessor = workflowRunProcessor;
            _gitHibActionsApiClient = gitHibActionsApiClient;
        }

        public async Task Run(string repoOwner, string repoName, long workflowId)
        {
            // hardcode the workflowId for now
            var pageNumber = 1;
            var resultsPerPage = 10;

            // first get the list of workflow runs from the API client
            // we're going to have to improve this and deal with paging etc
            var workflowRuns = await _gitHibActionsApiClient.GetWorkflowRuns(repoOwner, repoName, workflowId, DateTime.Now.AddDays(-2), pageNumber, resultsPerPage);

            // then process each workflow run
            foreach(var workflowRun in workflowRuns.workflow_runs)
            {
                await _workflowRunProcessor.Process(repoOwner, repoName, workflowRun);
            }
        }
    }
}
