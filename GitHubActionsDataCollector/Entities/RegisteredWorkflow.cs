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
    }
}
