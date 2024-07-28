namespace GitHubActionsDataCollector.GitHubActionsApiClient
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "dto object")]
    public class WorkflowRunListDto
    {
        public int total_count { get; set; }
        public List<WorkflowRunDto> workflow_runs { get; set; }
    }
}
