using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace GitHubActionsDataCollector.Services
{
    public interface IWorkflowRunLogsService
    {
        Task<string> GetRunAttemptLogForJob(string owner, string repo, string token, WorkflowRunJob job);
    }

    public class WorkflowRunLogsService : IWorkflowRunLogsService
    {
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;
        private ConcurrentDictionary<string, ZipArchive> _archives = new ConcurrentDictionary<string, ZipArchive>();

        public WorkflowRunLogsService(IGitHubActionsApiClient gitHubActionsApiClient)
        {
            _gitHubActionsApiClient = gitHubActionsApiClient;
        }

        public async Task<string> GetRunAttemptLogForJob(string owner, string repo, string token, WorkflowRunJob job)
        {
            var archive = await GetRunAttemptLogArtifact(owner, repo, token, job.RunId, job.RunAttempt);
            // full name contains the job details (folder name)
            // name needs to contain _Run Cypress.txt
            var entry = archive.Entries.FirstOrDefault(e => e.FullName.StartsWith(GetArchiveEntryPrefix(job), StringComparison.InvariantCultureIgnoreCase) 
                                                        && e.Name.EndsWith(GetArchiveEntrySuffix(job), StringComparison.InvariantCultureIgnoreCase));

            if (entry == null)
            {
                Console.WriteLine($"Could not find archive entry for job: {job.Id}");
                return string.Empty;
            }

            using (var sr = new StreamReader(entry.Open()))
            {
                var text = await sr.ReadToEndAsync();

                return RemoveAnsiEscapeCodes(text);
            }
        }

        private async Task<ZipArchive> GetRunAttemptLogArtifact(string owner, string repo, string token, long workflowRunId, int attemptNumber)
        {
            var key = GetKey(workflowRunId, attemptNumber);

            if (_archives.TryGetValue(key, out ZipArchive archive))
            {
                return archive;
            }

            var runAttemptLogsStream = await _gitHubActionsApiClient.GetWorkflowRunAttemptLogs(owner, repo, token, workflowRunId, attemptNumber);
            archive = new ZipArchive(runAttemptLogsStream, ZipArchiveMode.Read, true);
                    
            _archives.TryAdd(key, archive);

            return archive;
        }

        private string GetArchiveEntryPrefix(WorkflowRunJob job)
        {
            return job.Name.Replace("/", "");
        }

        private string GetArchiveEntrySuffix(WorkflowRunJob job)
        {
            if(job.Name.Contains("cypress", StringComparison.InvariantCultureIgnoreCase))
            {
                return "_Run Cypress.txt";
            }

            return string.Empty;
        }

        // TODO: this assumes lifetime of this object will be per run
        private string GetKey(long workflowRunId, int attemptNumber)
        {
            return $"{workflowRunId}-{attemptNumber}";
        }

        private string RemoveAnsiEscapeCodes(string input)
        {
            // Regex pattern to match ANSI escape codes
            string ansiEscapeCodePattern = @"\x1b\[[0-9;]*m";

            // Replace ANSI escape codes with an empty string
            return Regex.Replace(input, ansiEscapeCodePattern, string.Empty);
        }
    }
}
