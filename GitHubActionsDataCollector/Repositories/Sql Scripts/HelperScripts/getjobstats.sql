
SELECT	Durations.Name, 
		Durations.AvgDurationMins,
		ISNULL(Failures.FailureCount, 0) AS FailureCount,
		ISNULL(Successes.SuccessCount, 0) AS SuccessCount,
		ISNULL(Failures.FailureCount, 0)*100/(ISNULL(Failures.FailureCount, 0) + ISNULL(Successes.SuccessCount, 0)) AS FailurePercentage
FROM
(select Name, AVG(DATEDIFF(minute, StartedAtUtc, CompletedAtUtc)) AS AvgDurationMins
FROM [GHAData].[dbo].[WorkflowRunJob]
group by Name) AS Durations
LEFT JOIN
(select Name, COUNT(Conclusion) AS FailureCount
FROM [GHAData].[dbo].[WorkflowRunJob]
where Conclusion = 'failure'
group by Name, Conclusion) AS Failures
ON Failures.Name = Durations.Name
LEFT JOIN
(select Name, COUNT(Conclusion) AS SuccessCount
FROM [GHAData].[dbo].[WorkflowRunJob]
where Conclusion = 'success'
group by Name, Conclusion) AS Successes
ON Successes.Name = Durations.Name