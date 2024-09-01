using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.Repositories;
using System.IO.Compression;
using System.Xml.Linq;
using GitHubActionsDataCollector.Services;

namespace GitHubActionsDataCollector.Processors
{
    public interface IWorkflowRunJobsProcessor
    {
        public Task<List<WorkflowRunJob>> Process(string repoOwner, string repoName, string token, WorkflowRun workflowRun);
    }

    internal class WorkflowRunJobsProcessor : IWorkflowRunJobsProcessor
    {
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;
        private readonly IWorkflowRunJobProcessor _workflowRunJobProcessor;

        public WorkflowRunJobsProcessor(IGitHubActionsApiClient gitHubActionsApiClient,
                                        IWorkflowRunJobProcessor workflowRunJobProcessor)
        {
            _gitHubActionsApiClient = gitHubActionsApiClient;
            _workflowRunJobProcessor = workflowRunJobProcessor;
        }

        public async Task<List<WorkflowRunJob>> Process(string repoOwner, string repoName, string token, WorkflowRun workflowRun)
        {
            // we need to get the archive files for this run. they might be needed in job processing
            var artifactFiles = await _gitHubActionsApiClient.GetArtifactsListforWorkflowRun(repoOwner, repoName, token, workflowRun.RunId);

            var workflowJobs = new List<WorkflowRunJob>();
            var resultsPerPage = 40;
            var pageNumber = 0;
            // we use the jobIndex to keep track of which jobs we have already seen (some jobs seem to be captured multiple times, even though
            // they are only run once). We can use {name}-{started_at}-{completed_at} as the key to ensure they are unique. This ensures we capture
            // each unique occurence of a job
            var jobIndex = new Dictionary<string, long>();
            int totalResults;

            // it needs to page through the entire list of jobs for the workflow run. This might require multiple api calls.
            // then it needs to deduplicate jobs between different run attempts. For some reason, the same job can appear in
            // multiple run attempts even though it passed in an earlier run attempt.
            do
            {
                pageNumber++;

                var workflowRunJobs = await _gitHubActionsApiClient.GetJobsForWorkflowRun(repoOwner, repoName, token, workflowRun.RunId, pageNumber, resultsPerPage);
                totalResults = workflowRunJobs.total_count;

                foreach (var job in workflowRunJobs.jobs)
                {
                    if (!ShouldProcessJob(job))
                    {
                        Console.WriteLine($"Skipping job: {job.id} {job.name}");
                        continue;
                    }

                    var jobKey = GenerateJobKey(job);

                    // don't add again if we've seen this job before
                    if (jobIndex.ContainsKey(jobKey)) continue;

                    var workflowRunJob = new WorkflowRunJob
                    {
                        RunId = job.run_id,
                        JobId = job.id,
                        RunAttempt = job.run_attempt,
                        Name = job.name,
                        Conclusion = job.conclusion,
                        Url = job.html_url,
                        StartedAtUtc = DateTime.Parse(job.started_at, null, System.Globalization.DateTimeStyles.RoundtripKind),
                        CompletedAtUtc = DateTime.Parse(job.completed_at, null, System.Globalization.DateTimeStyles.RoundtripKind),
                        WorkflowRun = workflowRun
                    };

                    jobIndex.Add(jobKey, job.id);

                    Console.WriteLine($"Found new job: {jobKey}");

                    await _workflowRunJobProcessor.Process(repoOwner, repoName, token, workflowRunJob, artifactFiles);

                    workflowJobs.Add(workflowRunJob);
                }
            }
            while (pageNumber * resultsPerPage < totalResults);

            // we need to get the logs for each run attempt separately using https://docs.github.com/en/rest/actions/workflow-runs?apiVersion=2022-11-28#download-workflow-run-attempt-logs

            return workflowJobs;
        }

        private string GenerateJobKey(WorkflowRunJobDto job)
        {
            return $"{job.name}-{job.started_at}-{job.completed_at}";
        }

        private bool ShouldProcessJob(WorkflowRunJobDto job)
        {
            if (string.Equals("skipped", job.conclusion, StringComparison.OrdinalIgnoreCase)
                || string.Equals("cancelled", job.conclusion, StringComparison.OrdinalIgnoreCase))
                return false;

            return true;
        }
    }
}
