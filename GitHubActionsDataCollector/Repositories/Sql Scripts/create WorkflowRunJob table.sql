CREATE TABLE WorkflowRunJob (
    Id int IDENTITY(1,1) PRIMARY KEY,
	WorkflowRunId bigint,
    JobId bigint,
    RunAttempt smallint,
    Name varchar(255),
    Conclusion varchar(20),
    Url varchar(255),
    StartedAtUtc datetime,
    CompletedAtUtc datetime
);