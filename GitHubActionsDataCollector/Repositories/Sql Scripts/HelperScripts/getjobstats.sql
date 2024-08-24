
SELECT	Durations.Name, 
		Durations.AvgDurationMins, 
		ISNULL(Failures.FailureCount, 0) AS FailureCount, 
		ISNULL(Successes.SuccessCount, 0) AS SuccessCount, 
		ISNULL(Failures.FailureCount, 0)*100/(ISNULL(Failures.FailureCount, 0) + ISNULL(Successes.SuccessCount, 0)) AS FailurePercentage
FROM
(
	SELECT	Name, 
			AVG(DATEDIFF(minute, StartedAtUtc, CompletedAtUtc)) AS AvgDurationMins
	FROM [GHAData].[dbo].[WorkflowRunJob]
	GROUP BY Name
) AS Durations
LEFT JOIN
(
	SELECT	Name,
			COUNT(Conclusion) AS FailureCount
	FROM	[GHAData].[dbo].[WorkflowRunJob]
	WHERE	Conclusion = 'failure'
	GROUP BY Name, Conclusion
) AS Failures
ON Failures.Name = Durations.Name
LEFT JOIN
(
	SELECT	Name,
			COUNT(Conclusion) AS SuccessCount
	FROM	[GHAData].[dbo].[WorkflowRunJob]
	WHERE	Conclusion = 'success'
	GROUP BY Name, Conclusion) AS Successes
ON Successes.Name = Durations.Name