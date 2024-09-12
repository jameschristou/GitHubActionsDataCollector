DECLARE @fromDate DATETIME = DATEADD(d, -7, '2024/09/04 14:00:00');

WITH runs AS (
	SELECT	RunId,
			Title,
			Url AS RunUrl,
			StartedAtUtc,
			CompletedAtUtc,
			DATEDIFF(minute, StartedAtUtc, CompletedAtUtc) AS DurationMinutes,
			NumAttempts,
			Conclusion
	FROM	[GHAData].[dbo].[WorkflowRun]
	WHERE	StartedAtUtc > @fromDate AND Conclusion in ('success', 'failure')
)

--SELECT	RunId,
--		Title,
--		RunUrl,
--		StartedAtUtc,
--		CompletedAtUtc,
--		DurationMinutes,
--		NumAttempts,
--		Conclusion
--FROM	runs
--ORDER BY StartedAtUtc DESC

-- stats
--  SELECT	AVG(DurationMinutes) AS AvgDurationForSuccessfulRuns FROM runs WHERE Conclusion = 'success'
SELECT	ROUND(AVG(CAST(NumAttempts AS FLOAT)), 2) AS AvgAttemptsPerSuccessfulRuns FROM runs WHERE Conclusion = 'success'
-- SELECT	Conclusion, COUNT(Conclusion) FROM runs GROUP BY Conclusion



