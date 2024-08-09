namespace GitHubActionsDataCollector.Entities
{
    public class WorkflowRun
    {
        public virtual int Id { get; protected set; }
        public virtual string Owner { get; set; }
        public virtual string Repo { get; set; }
        public virtual long RunId { get; set; }
        public virtual long WorkflowId { get; set; }
        public virtual string WorkflowName { get; set; }
        public virtual string Title { get; set; }
        public virtual string Url { get; set; }
        public virtual DateTime StartedAtUtc { get; set; }
        public virtual DateTime CompletedAtUtc { get; set; }
        public virtual int NumAttempts { get; set; } // will this end up being a computed value instead?
        public virtual string Conclusion { get; set; }
    }
}
