DECLARE @toDate DATETIME = '2024/09/15 14:00:00';
DECLARE @fromDate DATETIME = DATEADD(d, -7, @toDate);

WITH jobs AS (
SELECT	Name,
		DATEDIFF(minute, StartedAtUtc, CompletedAtUtc) AS DurationMinutes,
		Conclusion
	FROM [GHAData].[dbo].[WorkflowRunJob]
	WHERE StartedAtUtc > @fromDate AND StartedAtUtc < @toDate)

SELECT	Durations.Name, 
		Durations.AvgDurationMinutes,
		Durations.MaxDurationMinutes, 
		Durations.MinDurationMinutes, 
		ISNULL(Failures.FailureCount, 0) AS FailureCount, 
		ISNULL(Successes.SuccessCount, 0) AS SuccessCount, 
		ISNULL(Failures.FailureCount, 0)*100/(ISNULL(Failures.FailureCount, 0) + ISNULL(Successes.SuccessCount, 0)) AS FailurePercentage
FROM
(
	SELECT	Name, 
			AVG(DurationMinutes) AS AvgDurationMinutes,
			MAX(DurationMinutes) AS MaxDurationMinutes,
			MIN(DurationMinutes) AS MinDurationMinutes
	FROM jobs
	GROUP BY Name
) AS Durations
LEFT JOIN
(
	SELECT	Name,
			COUNT(Conclusion) AS FailureCount
	FROM	jobs
	WHERE	Conclusion = 'Failure'
	GROUP BY Name, Conclusion
) AS Failures
ON Failures.Name = Durations.Name
LEFT JOIN
(
	SELECT	Name,
			COUNT(Conclusion) AS SuccessCount
	FROM	jobs
	WHERE	Conclusion = 'Success'
	GROUP BY Name, Conclusion) AS Successes
ON Successes.Name = Durations.Name
ORDER BY MinDurationMinutes desc