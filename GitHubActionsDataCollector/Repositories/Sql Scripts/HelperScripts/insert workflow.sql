USE GHAData
GO

INSERT INTO dbo.RegisteredWorkflow (
    Owner,
    Repo,
    WorkflowId,
    WorkflowName,
    LastCheckedAtUtc,
	ProcessedUntilUtc,
	Settings
)
VALUES
(
	'jameschristou',
	'GitHubActionsDataCollector',
	111639860,
	'build',
	'',
	GETUTCDATE(),
	'{
    "token": "your_token",
    "jobNameRequiredForRunSuccess": "build",
    "jobProcessingSettings":[
        {
            "matchingType": "Regex",
            "matchString": "Test",
            "processorName": "DotNetXmlTestResultsProcessor"
        }
    ]
}'
)