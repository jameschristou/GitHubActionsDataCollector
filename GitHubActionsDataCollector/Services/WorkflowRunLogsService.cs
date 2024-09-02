using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace GitHubActionsDataCollector.Services
{
    public interface IWorkflowRunLogsService
    {
        Task<ZipArchiveEntry> GetRunAttemptLogForJob(string owner, string repo, string token, WorkflowRunJob job);
    }

    public class WorkflowRunLogsService : IWorkflowRunLogsService
    {
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;
        private ConcurrentDictionary<string, ZipArchive> _archives = new ConcurrentDictionary<string, ZipArchive>();

        public WorkflowRunLogsService(IGitHubActionsApiClient gitHubActionsApiClient)
        {
            _gitHubActionsApiClient = gitHubActionsApiClient;
        }

        public async Task<ZipArchiveEntry> GetRunAttemptLogForJob(string owner, string repo, string token, WorkflowRunJob job)
        {
            var archive = await GetRunAttemptLogArtifact(owner, repo, token, job.RunId, job.RunAttempt);
            // full name contains the job details (folder name)
            // name needs to contain _Run Cypress.txt
            var archiveEntryPrefix = GetArchiveEntryPrefix(job);
            var archiveEntrySuffix = GetArchiveEntrySuffix(job);

            // NOTE: the archive entry names are truncated by GHA if too long so it may not be possible to find them

            var archiveEntries = archive.Entries.Where(e => e.FullName.StartsWith(archiveEntryPrefix, StringComparison.InvariantCultureIgnoreCase) 
                                                        && e.Name.Contains(archiveEntrySuffix, StringComparison.InvariantCultureIgnoreCase));

            if(archiveEntries == null || archiveEntries.Count() == 0)
            {
                //Regression1 migrate and test  Run smoke and regression tests Smoke Test Cypress Smoke Te(2)/ 4_Checkout.txt
                //Regression1 migrate and test / Run smoke and regression tests / Smoke Test / Cypress Smoke Test (en-AU, au-tests., 2)

                //Regression1 migrate and test  Run smoke and regression tests  Smoke Test  Cypress Smoke Te (1)/17_Post Checkout.txt
                Console.WriteLine($"Archive entry not found for job:{job.Id} name:{job.Name}");
                return null;
            }

            if(archiveEntries.Count() > 1)
            {
                Console.WriteLine($"Multiple archive entries found for job:{job.Id} name:{job.Name}");
                return null;
            }

            return archiveEntries.First();
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
                return "_Run Cypress";
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
