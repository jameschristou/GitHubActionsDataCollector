-- RUN STATS----
SELECT	RunId,
		Title,
		Url AS RunUrl,
		StartedAtUtc,
		CompletedAtUtc,
		DATEDIFF(minute, StartedAtUtc, CompletedAtUtc) AS DurationMinutes,
		NumAttempts,
		Conclusion
FROM	[GHAData].[dbo].[WorkflowRun]
WHERE	Conclusion in ('success', 'failure')
ORDER BY id DESC


-- TEST STATS----
SELECT	Durations.Name, 
		AvgDurationSeconds, 
		ISNULL(Failures.FailureCount, 0) AS FailureCount, 
		ISNULL(Successes.SuccessCount, 0) AS SuccessCount, 
		ISNULL(Failures.FailureCount, 0)*100/(ISNULL(Failures.FailureCount, 0) + ISNULL(Successes.SuccessCount, 0)) AS FailurePercentage
FROM
(
	SELECT	Name, 
			AVG(DurationMs)/1000 AS AvgDurationSeconds
	FROM [GHAData].[dbo].[TestResult]
	GROUP BY Name
) AS Durations
LEFT JOIN
(
	SELECT	Name,
			COUNT(Result) AS FailureCount
	FROM	[GHAData].[dbo].[TestResult]
	WHERE	Result = 'Failed'
	GROUP BY Name, Result
) AS Failures
ON Failures.Name = Durations.Name
LEFT JOIN
(
	SELECT	Name,
			COUNT(Result) AS SuccessCount
	FROM	[GHAData].[dbo].[TestResult]
	WHERE	Result = 'Passed'
	GROUP BY Name, Result) AS Successes
ON Successes.Name = Durations.Name
ORDER BY FailurePercentage DESC