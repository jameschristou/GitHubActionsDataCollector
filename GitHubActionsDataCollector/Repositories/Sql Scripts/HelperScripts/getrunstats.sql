SELECT RunId, Title, Url AS RunUrl, StartedAtUtc, CompletedAtUtc, DATEDIFF(minute, StartedAtUtc, CompletedAtUtc) AS DurationMinutes, NumAttempts, Conclusion
  FROM [GHAData].[dbo].[WorkflowRun]
 where Conclusion in ('success', 'failure')
  order by id desc