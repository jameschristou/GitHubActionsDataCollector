using System.Text.Json;

namespace GitHubActionsDataCollector.Entities
{
    public class RegisteredWorkflow
    {
        private string _settings;
        private WorkflowRunSettings _workflowRunSettings;

        public virtual int Id { get; protected set; }
        public virtual string Owner { get; set; }
        public virtual string Repo { get; set; }
        public virtual long WorkflowId { get; set; }
        public virtual string WorkflowName { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual string Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;

                if (string.IsNullOrEmpty(_settings))
                {
                    _workflowRunSettings = new WorkflowRunSettings();
                }
                else
                {
                    _workflowRunSettings = JsonSerializer.Deserialize<WorkflowRunSettings>(_settings, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
        }
        public virtual DateTime LastCheckedAtUtc { get; set; }
        // this is the date we have checked workflow runs until. When we check this workflow again, we should check from this date
        // we can't rely on checking the last WorkflowRun because there might not be any runs for the period
        public virtual DateTime ProcessedUntilUtc { get; set; }

        public virtual WorkflowRunSettings GetSettings()
        {
            return _workflowRunSettings;
        }
    }
}
