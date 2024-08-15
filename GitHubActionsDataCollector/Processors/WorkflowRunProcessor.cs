using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.Repositories;

namespace GitHubActionsDataCollector.Processors
{
    public interface IWorkflowRunProcessor
    {
        public Task Process(RegisteredWorkflow registeredWorkflow, WorkflowRunDto workflowRun);
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

        public async Task Process(RegisteredWorkflow registeredWorkflow, WorkflowRunDto workflowRunDto)
        {
            var createdAt = DateTime.Parse(workflowRunDto.created_at, null, System.Globalization.DateTimeStyles.RoundtripKind);
            var updatedAt = DateTime.Parse(workflowRunDto.updated_at, null, System.Globalization.DateTimeStyles.RoundtripKind);

            var duration = updatedAt.Subtract(createdAt);

            Console.WriteLine($"WorkflowRunId:{workflowRunDto.id} Title:{workflowRunDto.display_title} Duration:{duration} RunAttempts:{workflowRunDto.run_attempt} Conclusion:{workflowRunDto.conclusion}");

            if (!ShouldProcessWorkflowRun(workflowRunDto))
            {
                Console.WriteLine($"Skipping WorkflowRunId:{workflowRunDto.id}");
                return;
            }

            var workflowRun = new WorkflowRun
            {
                Owner = registeredWorkflow.Owner,
                Repo = registeredWorkflow.Repo,
                RunId = workflowRunDto.id,
                WorkflowId = workflowRunDto.workflow_id,
                WorkflowName = workflowRunDto.name,
                StartedAtUtc = createdAt,
                Title = workflowRunDto.display_title,
                Conclusion = workflowRunDto.conclusion,
                Url = workflowRunDto.html_url,
                NumAttempts = workflowRunDto.run_attempt
            };

            var jobs = await _workflowRunJobsProcessor.Process(registeredWorkflow.Owner, registeredWorkflow.Repo, registeredWorkflow.Token, workflowRun);

            workflowRun.Jobs = jobs;

            if (ShouldSaveWorkflowRun(workflowRun))
            {
                workflowRun.CompletedAtUtc = GetWorkflowRunCompletionTime(jobs);
                workflowRun.ProcessedAtUtc = DateTime.UtcNow;

                await _workflowRunRepository.SaveWorkflowRun(workflowRun);
            }
        }

        private bool ShouldProcessWorkflowRun(WorkflowRunDto workflowRun)
        {
            if (string.Equals("success", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase)
                || string.Equals("failure", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase)
                || string.Equals("cancelled", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private bool ShouldSaveWorkflowRun(WorkflowRun workflowRun)
        {
            if (workflowRun.Jobs.Any(x => string.Equals("success", x.Conclusion, StringComparison.OrdinalIgnoreCase)
                                            || string.Equals("failure", x.Conclusion, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            // we don't save runs where no jobs had success or failure
            return false;
        }

        private DateTime GetWorkflowRunCompletionTime(List<WorkflowRunJob> workflowRunJobs)
        {
            // the github actions api doesn't give us a date that the workflow finished running
            // we get this by getting the completion time of the last job to complete or fail
            return workflowRunJobs.OrderBy(x => x.CompletedAtUtc).Last().CompletedAtUtc;
        }
    }
}
