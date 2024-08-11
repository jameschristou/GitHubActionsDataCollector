using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.Repositories;

namespace GitHubActionsDataCollector
{
    public interface IWorkflowRunProcessor
    {
        public Task Process(string repoOwner, string repoName, WorkflowRunDto workflowRun);
    }

    /**
     * Gets data for a workflow run and processes it:
     * Initially just write the data we're interested in to the console to ensure we're processing it correctly
     */
    internal class WorkflowRunProcessor : IWorkflowRunProcessor
    {
        private readonly IWorkflowRunJobsProcessor _workflowRunJobsProcessor;
        private readonly IWorkflowRunRepository _workflowRunRepository;
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;

        public WorkflowRunProcessor(IWorkflowRunJobsProcessor workflowRunJobsProcessor,
                                    IWorkflowRunRepository workflowRunRepository,
                                    IGitHubActionsApiClient gitHubActionsApiClient)
        {
            _workflowRunJobsProcessor = workflowRunJobsProcessor;
            _workflowRunRepository = workflowRunRepository;
            _gitHubActionsApiClient = gitHubActionsApiClient;
        }

        public async Task Process(string owner, string repo, WorkflowRunDto workflowRunDto)
        {
            var createdAt = DateTime.Parse(workflowRunDto.created_at);
            var updatedAt = DateTime.Parse(workflowRunDto.updated_at);

            var duration = updatedAt.Subtract(createdAt);

            Console.WriteLine($"WorkflowRunId:{workflowRunDto.id} Title:{workflowRunDto.display_title} Duration:{duration} RunAttempts:{workflowRunDto.run_attempt} Conclusion:{workflowRunDto.conclusion}");

            if (!ShouldProcessWorkflowRun(workflowRunDto))
            {
                Console.WriteLine($"Skipping WorkflowRunId:{workflowRunDto.id}");
                return;
            }

            var workflowRun = new WorkflowRun
            {
                Owner = owner,
                Repo = repo,
                RunId = workflowRunDto.id,
                WorkflowId = workflowRunDto.workflow_id,
                WorkflowName = workflowRunDto.name,
                CompletedAtUtc = updatedAt,
                StartedAtUtc = createdAt,
                Title = workflowRunDto.display_title,
                Conclusion = workflowRunDto.conclusion,
                Url = workflowRunDto.html_url,
                NumAttempts = workflowRunDto.run_attempt
            };

            var jobs = await _workflowRunJobsProcessor.Process(owner, repo, workflowRun);

            workflowRun.Jobs = jobs;

            _workflowRunRepository.SaveWorkflowRun(workflowRun);
        }

        private bool ShouldProcessWorkflowRun(WorkflowRunDto workflowRun)
        {
            if (String.Equals("success", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
