using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.Repositories;

namespace GitHubActionsDataCollector.Processors
{
    public interface IWorkflowRunJobsProcessor
    {
        public Task<List<WorkflowRunJob>> Process(string repoOwner, string repoName, string token, WorkflowRun workflowRun);
    }

    internal class WorkflowRunJobsProcessor : IWorkflowRunJobsProcessor
    {
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;
        private readonly IWorkflowRunJobsRepository _workflowRunJobsRepository;

        public WorkflowRunJobsProcessor(IGitHubActionsApiClient gitHubActionsApiClient,
                                        IWorkflowRunJobsRepository workflowRunJobsRepository)
        {
            _gitHubActionsApiClient = gitHubActionsApiClient;
            _workflowRunJobsRepository = workflowRunJobsRepository;
        }

        public async Task<List<WorkflowRunJob>> Process(string repoOwner, string repoName, string token, WorkflowRun workflowRun)
        {
            var workflowJobs = new List<WorkflowRunJob>();
            var resultsPerPage = 30;
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

                    workflowJobs.Add(
                        new WorkflowRunJob
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
                        }
                    );

                    jobIndex.Add(jobKey, job.id);

                    Console.WriteLine($"Found new job: {jobKey}");
                }
            }
            while (pageNumber * resultsPerPage < totalResults);

            // now we can save the jobs to the DB

            // we can then pull down the logs for each workflow run attempt. These will be received as a zip file.
            // we'll need to processs this file and look for the test results in each log
            // we'll need to be able to configure which jobs and which steps in each job contain test logs and what type of logs these are
            // or we can allow the user to write a custom class to deal with these
            // _workflowRunJobsRepository.SaveJobs(workflowJobs);

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
