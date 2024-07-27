﻿namespace GitHubActionsDataCollector.GHADtos
{
    internal class WorkflowRunDto
    {
        public int id { get; set; }
        public string title { get; set; }
        public string conclusion { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public int run_attempt { get; set; }
        public string html_url { get; set; }
    }
}
