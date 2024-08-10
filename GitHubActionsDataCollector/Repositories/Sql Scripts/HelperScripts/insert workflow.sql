INSERT INTO dbo.RegisteredWorkflow (
    Owner,
    Repo,
    WorkflowId,
    WorkflowName,
    Token,
    LastCheckedAtUtc
)
VALUES
(
	'jameschristou',
	'GitHubActionsDataCollector',
	111639860,
	'build',
	'',
	GETUTCDATE()
)