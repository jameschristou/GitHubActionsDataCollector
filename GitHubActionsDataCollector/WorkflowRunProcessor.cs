using GitHubActionsDataCollector.GitHubActionsApiClient;

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
        public void Process(WorkflowRunDto workflowRun)
        {
            var createdAt = DateTime.Parse(workflowRun.created_at);
            var updatedAt = DateTime.Parse(workflowRun.updated_at);

            var duration = updatedAt.Subtract(createdAt);

            Console.WriteLine($"WorkflowRunId:{workflowRun.id} Title:{workflowRun.title} Duration:{duration} RunAttempts:{workflowRun.run_attempt} Conclusion:{workflowRun.conclusion}");
        }

        private bool ShouldProcessWorkflowRun(WorkflowRunDto workflowRun)
        {
            if (String.Equals("success", workflowRun.conclusion, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }
    }
}
