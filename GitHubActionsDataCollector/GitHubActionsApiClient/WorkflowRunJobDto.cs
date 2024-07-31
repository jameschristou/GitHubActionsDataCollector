namespace GitHubActionsDataCollector.GitHubActionsApiClient
{
    public class WorkflowRunJobDto
    {
        public long id { get; set; }
        public string name { get; set; }
        public string conclusion { get; set; }
        public string started_at { get; set; }
        public string completed_at { get; set; }
        public long run_id { get; set; }
        public string html_url { get; set; }
        public int run_attempt { get; set; }
    }
}
