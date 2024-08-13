namespace GitHubActionsDataCollector.Entities
{
    public class RegisteredWorkflow
    {
        public virtual int Id { get; protected set; }
        public virtual string Owner { get; set; }
        public virtual string Repo { get; set; }
        public virtual long WorkflowId { get; set; }
        public virtual string WorkflowName { get; set; }
        public virtual string Token { get; set; }
        public virtual DateTime LastCheckedAtUtc { get; set; }
        // this is the date we have checked workflow runs until. When we check this workflow again, we should check from this date
        // we can't rely on checking the last WorkflowRun because there might not be any runs for the period
        public virtual DateTime ProcessedUntilUtc { get; set; }
    }
}
