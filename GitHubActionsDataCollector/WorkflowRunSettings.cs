namespace GitHubActionsDataCollector
{
    public class WorkflowRunSettings
    {
        public string Token { get; set; }

        // if this job name succeeds then that means the workflow run has succeeded regardless of the run conclusion
        // leave this blank to just rely on the run conclusion
        public string JobNameRequiredForRunSuccess { get; set; }

        public List<JobProcessingSetting> JobProcessingSettings { get; set; }
    }

    public class JobProcessingSetting
    {
        public JobProcessingMatchingType MatchingType { get; set; }
        public string MatchString { get; set; }
        public string ProcessorName { get; set; }
    }

    public enum JobProcessingMatchingType
    {
        Regex,
        Exact
    }
}
