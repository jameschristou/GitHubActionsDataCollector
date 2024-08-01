using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Models;
using GitHubActionsDataCollector.Repository;

namespace GitHubActionsDataCollector
{
    public interface IWorkflowRunProcessor
    {
        public void Process(WorkflowRunDto workflowRun);
    }

    /**
     * Gets data for a workflow run and processes it:
     * Initially just write the data we're interested in to the console to ensure we're processing it correctly
     */
    internal class WorkflowRunProcessor : IWorkflowRunProcessor
    {
        private readonly IWorkflowRunRepository _workflowRunRepository;
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;

        public WorkflowRunProcessor(IWorkflowRunRepository workflowRunRepository,
                                    IGitHubActionsApiClient gitHubActionsApiClient)
        {
            _workflowRunRepository = workflowRunRepository;
            _gitHubActionsApiClient = gitHubActionsApiClient;
        }

        public void Process(WorkflowRunDto workflowRun)
        {
            var createdAt = DateTime.Parse(workflowRun.created_at);
            var updatedAt = DateTime.Parse(workflowRun.updated_at);

            var duration = updatedAt.Subtract(createdAt);

            Console.WriteLine($"WorkflowRunId:{workflowRun.id} Title:{workflowRun.title} Duration:{duration} RunAttempts:{workflowRun.run_attempt} Conclusion:{workflowRun.conclusion}");

            if (!ShouldProcessWorkflowRun(workflowRun))
            {
                Console.WriteLine($"Skipping WorkflowRunId:{workflowRun.id}");
                return;
            }

            _workflowRunRepository.SaveWorkflowRun(new WorkflowRunModel
            {
                Id = workflowRun.id,
                DateCompletedUtc = updatedAt,
                DateStartedUtc = createdAt,
                Title = workflowRun.title,
                Conclusion = workflowRun.conclusion,
                Url = workflowRun.html_url,
                NumAttempts = workflowRun.run_attempt
            });

            GetWorkflowRunJobs(workflowRun.id);
        }

        private bool ShouldProcessWorkflowRun(WorkflowRunDto workflowRun)
        {
            if (String.Equals("success", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        private void GetWorkflowRunJobs(long id)
        {
            var workflowJobs = new List<WorkflowRunJobModel>();
            var resultsPerPage = 4;

            // we might need to move this function into a service since it's doing quite a lot.
            // it needs to page through the entire list of jobs for the workflow run. This might require multiple api calls.
            // then it needs to deduplicate jobs between different run attempts. For some reason, the same job can appear in
            // multiple run attempts even though it passed in an earlier run attempt.

            var totalResults = 0;
            var pageNumber = 0;
            // we use the jobIndex to keep track of which jobs we have already seen (some jobs seem to be captured multiple times, even though
            // they are only run once). We can use {name}-{started_at}-{completed_at} as the key to ensure they are unique
            var jobIndex = new Dictionary<string, long>();

            do
            {
                pageNumber++;

                var workflowRunJobs = _gitHubActionsApiClient.GetJobsForWorkflowRun(id, pageNumber);
                totalResults = workflowRunJobs.total_count;

                foreach ( var job in workflowRunJobs.jobs)
                {
                    var jobKey = GenerateJobKey(job);

                    // don't add again if we've seen this job before
                    if (jobIndex.ContainsKey(jobKey)) continue;

                    workflowJobs.Add(
                        new WorkflowRunJobModel
                        {
                            Id = job.id
                        }
                    );

                    jobIndex.Add(jobKey, job.id);

                    Console.WriteLine($"Found new job: {jobKey}");
                }
            }
            while (pageNumber * resultsPerPage < totalResults);
        }

        private string GenerateJobKey(WorkflowRunJobDto job)
        {
            return $"{job.name}-{job.started_at}-{job.completed_at}";
        }
    }
}
