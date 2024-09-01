using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Services;
using System.Text.RegularExpressions;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public class CypressTestResultsProcessor : IJobProcessor
    {
        private readonly IWorkflowRunLogsService _workflowRunLogsService;

        public CypressTestResultsProcessor(IWorkflowRunLogsService workflowRunLogsService)
        {
            _workflowRunLogsService = workflowRunLogsService;
        }

        public async Task Process(string owner, string repo, string token, WorkflowRunJob job, WorkflowRunArtifactsDto artifacts)
        {
            var testLog = await _workflowRunLogsService.GetRunAttemptLogForJob(owner, repo, token, job);

            // we get this through the job name
            var regEx = new Regex(@"API regression test \(([^,]*),");
            var matches = regEx.Matches(job.Name);
            if (matches.Any())
            {
                var test = matches[0].Groups[1].Value;
            }

            return;
        }

        public static bool CanProcessJob(WorkflowRunJob job)
        {
            return job.Name.Contains("cypress", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
