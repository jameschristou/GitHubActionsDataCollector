using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Services;
using System.Text.RegularExpressions;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public class CypressTestResultsProcessor : IJobProcessor
    {
        private readonly IWorkflowRunLogsService _workflowRunLogsService;

        public CypressTestResultsProcessor(IWorkflowRunLogsService workflowRunLogsService)
        {
            _workflowRunLogsService = workflowRunLogsService;
        }

        public async Task Process(string owner, string repo, string token, WorkflowRunJob job, WorkflowRunArtifactsDto artifacts)
        {
            var entry = await _workflowRunLogsService.GetRunAttemptLogForJob(owner, repo, token, job);

            if (entry == null)
            {
                Console.WriteLine($"Could not find logs for job:{job.Id} name:{job.Name}");
                return;
            }

            using (var sr = new StreamReader(entry.Open()))
            {
                try
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        //var cleanLine = RemoveAnsiEscapeCodes(line);

                        // in this next section we look for the summary table headings: Spec                                              Tests  Passing  Failing  Pending  Skipped
                        var skippedIndex = line.IndexOf("Skipped", StringComparison.InvariantCultureIgnoreCase);
                        if (skippedIndex == -1) { continue; }

                        var pendingIndex = line.IndexOf("Pending", StringComparison.InvariantCultureIgnoreCase);
                        if (pendingIndex == -1) { continue; }

                        var failingIndex = line.IndexOf("Failing", StringComparison.InvariantCultureIgnoreCase);
                        if (failingIndex == -1) { continue; }

                        var passingIndex = line.IndexOf("Passing", StringComparison.InvariantCultureIgnoreCase);
                        if (passingIndex == -1) { continue; }

                        var testsIndex = line.IndexOf("Tests", StringComparison.InvariantCultureIgnoreCase);
                        if (testsIndex == -1) { continue; }

                        var specIndex = line.IndexOf("Spec", StringComparison.InvariantCultureIgnoreCase);
                        if (specIndex == -1) { continue; }

                        if(specIndex > testsIndex || testsIndex > passingIndex || passingIndex > failingIndex || failingIndex > pendingIndex || pendingIndex > skippedIndex)
                        {
                            continue;
                        }

                        // ok we've found the test results summary. Let's process them!
                        var testResults = await GetTestResultsFromTestResultsSummary(sr);

                        if(testResults != null && testResults.Any())
                        {
                            testResults.ForEach(t => t.WorkflowRunJob = job);
                            job.TestResults = testResults;
                        }

                        break;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return;
        }

        private async Task<List<TestResult>> GetTestResultsFromTestResultsSummary(StreamReader sr)
        {
            var testResults = new List<TestResult>();

            // we throw away the first line which is the table border
            var line = await sr.ReadLineAsync();

            while ((line = await ReadLineWithAnsiEscapeCodesRemoved(sr)) != null)
            {
                if (line.Contains('├'))
                {
                    // this is the row border so skip it
                    continue;
                }

                if (!line.Contains('│'))
                {
                    // we've finished processing the summary
                    break;
                }

                // we have summary for a spec so process this
                var result = await ProcessSpecSummary(line, sr);

                if(result != null)
                {
                    testResults.Add(result);
                }
            }

            return testResults;
        }

        private async Task<TestResult> ProcessSpecSummary(string currentLine, StreamReader sr)
        {
            var regEx = new Regex(@"^(.*?)[│] ([✔|✖])  (.\S*)(\s*)([:\d]*)(ms)?(\s*)([\-:\d]*)(\s*)([\-:\d]*)(\s*)([\-:\d]*)");
            var matches = regEx.Matches(currentLine);
            if (!matches.Any())
            {
                return null;
            }
            
            var testName = matches[0].Groups[3].Value;
            var testDuration = matches[0].Groups[5].Value;
            var testsFailed = matches[0].Groups[12].Value;

            if (!testName.Contains(".js") && !testName.Contains(".ts"))
            {
                var continueGettingTestName = true;
                // if the spec name doesn't contain .js then we keep looking in the next line
                while (continueGettingTestName)
                {
                    var line = await ReadLineWithAnsiEscapeCodesRemoved(sr);

                    var regExRemainingName = new Regex(@"^(.*?)[│](\s*)(.\S*)");
                    var matchesRemainingName = regExRemainingName.Matches(line);

                    if (!matchesRemainingName.Any() || matchesRemainingName[0].Groups.Count < 4)
                    {
                        Console.WriteLine($"Could not find the rest of the test name. Current testname:{testName} Current line:{line}");
                        break;
                    }

                    testName += matchesRemainingName[0].Groups[3].Value;

                    if (testName.Contains(".js"))
                    {
                        break;
                    }
                }
            }

            return new TestResult()
            {
                Name = testName,
                DurationMs = GetDurationInMs(testDuration),
                Result = testsFailed == "-" ? "Passed" : "Failed"
            };
        }

        private int GetDurationInMs(string durationString)
        {
            var duration = 0;

            if (durationString.Contains(':'))
            {
                var durationParts = durationString.Split(':').Select(x => int.Parse(x)).ToList();
                duration = durationParts[0] * 60 * 1000 + durationParts[1] * 1000;
            }
            else // milliseconds only
            {
                duration = int.Parse(durationString);
            }

            return duration;
        }

        public static bool CanProcessJob(WorkflowRunJob job)
        {
            return job.Name.Contains("cypress", StringComparison.InvariantCultureIgnoreCase);
        }

        private string RemoveAnsiEscapeCodes(string input)
        {
            // Regex pattern to match ANSI escape codes
            string ansiEscapeCodePattern = @"\x1b\[[0-9;]*m";

            // Replace ANSI escape codes with an empty string
            return Regex.Replace(input, ansiEscapeCodePattern, string.Empty);
        }

        private async Task<string> ReadLineWithAnsiEscapeCodesRemoved(StreamReader sr)
        {
            var line = await sr.ReadLineAsync();

            if (line == null) return null;

            return RemoveAnsiEscapeCodes(line);
        }
    }
}
