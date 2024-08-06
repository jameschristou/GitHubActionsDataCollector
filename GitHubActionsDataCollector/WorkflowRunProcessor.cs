using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Models;
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

        public async Task Process(string repoOwner, string repoName, WorkflowRunDto workflowRun)
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
                RunId = workflowRun.id,
                WorkflowId = workflowRun.workflow_id,
                WorkflowName = workflowRun.name,
                CompletedAtUtc = updatedAt,
                StartedAtUtc = createdAt,
                Title = workflowRun.title,
                Conclusion = workflowRun.conclusion,
                Url = workflowRun.html_url,
                NumAttempts = workflowRun.run_attempt
            });

            await _workflowRunJobsProcessor.Process(repoOwner, repoName, workflowRun.id);
        }

        private bool ShouldProcessWorkflowRun(WorkflowRunDto workflowRun)
        {
            if (String.Equals("success", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
