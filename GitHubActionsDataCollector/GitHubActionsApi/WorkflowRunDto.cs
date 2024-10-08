﻿namespace GitHubActionsDataCollector.GitHubActionsApi
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "dto object")]
    public class WorkflowRunDto
    {
        public long id { get; set; }
        public long workflow_id { get; set; }
        public string name { get; set; }
        public string display_title { get; set; }
        public string conclusion { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int run_attempt { get; set; }
        public string html_url { get; set; }
    }
}