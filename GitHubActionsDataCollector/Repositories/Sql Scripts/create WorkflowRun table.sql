CREATE TABLE WorkflowRun (
    Id int,
	RunId bigint,
    WorkflowId bigint,
    WorkflowName varchar(255),
    Title varchar(255),
    Url varchar(255),
    StartedAtUtc datetime,
    CompletedAtUtc datetime,
    NumAttempts smallint,
    Conclusion varchar(20)
);