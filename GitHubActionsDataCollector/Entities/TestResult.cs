namespace GitHubActionsDataCollector.Entities
{
    public class TestResult
    {
        public virtual long Id { get; protected set; }
        public virtual string Name { get; set; }
        public virtual string Result { get; set; }
        public virtual int DurationMs { get; set; }
        public virtual WorkflowRunJob WorkflowRunJob { get; set; }
    }
}
