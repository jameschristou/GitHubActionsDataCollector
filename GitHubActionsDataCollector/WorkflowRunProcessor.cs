using GitHubActionsDataCollector.GHADtos;

namespace GitHubActionsDataCollector
{
    /**
     * Gets data for a workflow run and processes it:
     * Initially just write the data we're interested in to the console to ensure we're processing it correctly
     */
    internal class WorkflowRunProcessor
    {
        public void Process(WorkflowRunDto workflowRun)
        {
            Console.WriteLine($"WorkflowRunId:{workflowRun.id}");
        }
    }
}
