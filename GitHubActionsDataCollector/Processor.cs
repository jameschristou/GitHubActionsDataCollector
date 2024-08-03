using GitHubActionsDataCollector.GitHubActionsApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task Run()
        {
            // first get the list of workflow runs from the API client
            var workflowRuns = _gitHibActionsApiClient.GetWorkflowRuns(DateTime.Now.AddDays(-2));

            // then process each workflow run
            foreach(var workflowRun in workflowRuns.workflow_runs)
            {
                await _workflowRunProcessor.Process(workflowRun);
            }
        }
    }
}
