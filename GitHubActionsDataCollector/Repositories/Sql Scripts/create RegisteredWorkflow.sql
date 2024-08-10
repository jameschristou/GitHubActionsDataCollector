CREATE TABLE RegisteredWorkflow (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Owner varchar(100),
    Repo varchar(100),
    WorkflowId bigint,
    WorkflowName varchar(255),
    Token varchar(100),
    LastCheckedAtUtc datetime
);