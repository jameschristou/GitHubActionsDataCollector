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
            var resultsPerPage = 1;

            // first get the list of workflow runs from the API client
            // we're going to have to improve this and deal with paging etc
            // TODO: we really need to be looping here to do the paging and get more workflow runs
            var workflowRuns = await _gitHibActionsApiClient.GetWorkflowRuns(repoOwner, repoName, workflowId, DateTime.Now.AddDays(-2), pageNumber, resultsPerPage);

            // then process each workflow run
            foreach(var workflowRun in workflowRuns.workflow_runs)
            {
                await _workflowRunProcessor.Process(repoOwner, repoName, workflowRun);
            }
        }
    }
}
