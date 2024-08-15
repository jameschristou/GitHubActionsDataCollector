namespace GitHubActionsDataCollector.GitHubActionsApi
{
    public class WorkflowRunJobsListDto
    {
        public int total_count { get; set; }
        public List<WorkflowRunJobDto> jobs { get; set; }
    }
}
