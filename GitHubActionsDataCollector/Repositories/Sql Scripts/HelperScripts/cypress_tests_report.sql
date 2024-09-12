-- script for getting cypress test report
DECLARE @toDate DATETIME = '2024/09/15 14:00:00';
DECLARE	@fromDate DATETIME = DATEADD(d, -7, @toDate),
		@fromDate4weeks DATETIME = DATEADD(d, -28, @toDate);

WITH test_results_last_week AS (
SELECT	REPLACE(REPLACE(wrj.Name, 'Staging / Regression Test / ', ''), 'Staging / Smoke Test / ', '') + ' ' + tr.Name AS Name,
		DurationMs/1000 AS DurationSeconds,
		Result,
		Url,
		StartedAtUtc,
		wrj.RunId
	FROM [GHAData].[dbo].[WorkflowRunJob] wrj
	JOIN [GHAData].[dbo].[TestResult] tr
	ON tr.WorkflowRunJobId = wrj.Id
	WHERE wrj.StartedAtUtc > @fromDate AND wrj.StartedAtUtc < @toDate AND wrj.Name LIKE '%cypress%'
		AND wrj.RunId NOT IN (10804295680) -- runId ignore list in case something went wrong with this run
	),

test_results_last_4weeks AS (
SELECT	REPLACE(REPLACE(wrj.Name, 'Staging / Regression Test / ', ''), 'Staging / Smoke Test / ', '') + ' ' + tr.Name AS Name,
		DurationMs/1000 AS DurationSeconds,
		Result,
		Url,
		StartedAtUtc
	FROM [GHAData].[dbo].[WorkflowRunJob] wrj
	JOIN [GHAData].[dbo].[TestResult] tr
	ON tr.WorkflowRunJobId = wrj.Id
	WHERE wrj.StartedAtUtc > @fromDate4weeks AND wrj.StartedAtUtc < @toDate AND wrj.Name LIKE '%cypress%'
		AND wrj.RunId NOT IN (10804295680) -- runId ignore list in case something went wrong with this run
	)

SELECT	Durations.Name, 
		Durations.AvgDurationSeconds,
		Durations.MaxDurationSeconds,
		Durations.MinDurationSeconds,
		ISNULL(Failures.FailureCount, 0) AS FailureCount, 
		ISNULL(Successes.SuccessCount, 0) AS SuccessCount,
		ISNULL(Failures.FailureCount, 0)*100/(ISNULL(Failures.FailureCount, 0) + ISNULL(Successes.SuccessCount, 0)) AS FailurePercentage,
		ISNULL(RunsImpacted.NumberOfRunsImpacted, 0) AS NumberOfRunsImpacted,
		ISNULL(FailuresLast4Weeks.FailureCount, 0) AS FailureCountLast4Weeks
FROM
(
	SELECT	Name,
			AVG(DurationSeconds) AS AvgDurationSeconds,
			MAX(DurationSeconds) AS MaxDurationSeconds,
			MIN(DurationSeconds) AS MinDurationSeconds
	FROM	test_results_last_week
	GROUP BY Name
) AS Durations
LEFT JOIN
(
	SELECT	Name,
			COUNT(Result) AS FailureCount
	FROM	test_results_last_week
	WHERE	Result = 'Failed'
	GROUP BY Name, Result
) AS Failures
ON Failures.Name = Durations.Name
LEFT JOIN
(
	SELECT	Name,
			COUNT(Result) AS SuccessCount
	FROM	test_results_last_week
	WHERE	Result = 'Passed'
	GROUP BY Name, Result
) AS Successes
ON Successes.Name = Durations.Name
LEFT JOIN
(
	SELECT	Name,
			COUNT(Name) AS NumberOfRunsImpacted
	FROM (
		SELECT	Name,
				COUNT(RunId) AS NumberOfRunsImpacted
		FROM	test_results_last_week
		WHERE	Result = 'Failed'
		GROUP BY Name, RunId
	) AS RunsImpacted
	GROUP BY Name
) AS RunsImpacted
ON RunsImpacted.Name = Durations.Name
LEFT JOIN
(
	SELECT	Name,
			COUNT(Result) AS FailureCount
	FROM	test_results_last_4weeks
	WHERE	Result = 'Failed'
	GROUP BY Name, Result
) AS FailuresLast4Weeks
ON FailuresLast4Weeks.Name = Durations.Name
ORDER by FailurePercentage desc

-- list of failures during this period
--DECLARE @toDate DATETIME = '2024/09/15 14:00:00';
--DECLARE	@fromDate DATETIME = DATEADD(d, -7, @toDate),
--		@fromDate4weeks DATETIME = DATEADD(d, -28, @toDate);

--SELECT	RunId,
--		RunAttempt,
--		REPLACE(REPLACE(wrj.Name, 'Staging / Regression Test / ', ''), 'Staging / Smoke Test / ', '') AS JobName,
--		tr.Name AS TestName,
--		Url AS JobUrl,
--		StartedAtUtc
--	FROM [GHAData].[dbo].[WorkflowRunJob] wrj
--	JOIN [GHAData].[dbo].[TestResult] tr
--	ON tr.WorkflowRunJobId = wrj.Id
--	WHERE wrj.StartedAtUtc > @fromDate AND wrj.StartedAtUtc < @toDate AND tr.Result = 'Failed' AND wrj.Name LIKE '%cypress%'
--	ORDER BY StartedAtUtc DESC