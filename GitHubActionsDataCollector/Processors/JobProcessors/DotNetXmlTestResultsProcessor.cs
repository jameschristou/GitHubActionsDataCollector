using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public class DotNetXmlTestResultsProcessor : IJobProcessor
    {
        private readonly IGitHubActionsApiClient _gitHubActionsApiClient;

        public DotNetXmlTestResultsProcessor(IGitHubActionsApiClient gitHubActionsApiClient)
        {
            _gitHubActionsApiClient = gitHubActionsApiClient;
        }

        public async Task Process(string owner, string repo, string token, WorkflowRunJob job, WorkflowRunArtifactsDto artifacts)
        {
            var categoryName = GetCategoryName(job);

            if(string.IsNullOrEmpty(categoryName))
            {
                Console.WriteLine($"Could not get category name for job:{job.Name}");
                return;
            }

            // first we need to get the artifact file for this job. Match of the test category, the run attempt and the environment
            var artifact = artifacts.artifacts.FirstOrDefault(a => a.name.Contains(categoryName, StringComparison.OrdinalIgnoreCase) &&
                                                                    a.name.Contains($"Attempt-{job.RunAttempt}", StringComparison.OrdinalIgnoreCase) &&
                                                                    a.name.Contains(GetEnvironmentName(job), StringComparison.OrdinalIgnoreCase));

            if(artifact == null)
            {
                Console.WriteLine($"Could not get artifact name for job:{job.Name}");
                return;
            }

            XDocument xdoc;

            // now we download the artifact, unzip it and load it into XML doc
            using (var artifactStream = await _gitHubActionsApiClient.GetWorkflowRunArtifact(owner, repo, token, artifact.id))
            {
                using (var archive = new ZipArchive(artifactStream, ZipArchiveMode.Read, true))
                {
                    var entry = archive.Entries.First();

                    using (StreamReader sr = new StreamReader(entry.Open()))
                    {
                        xdoc = XDocument.Load(sr);
                    }
                }
            }

            XNamespace ns = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";

            var results = xdoc.Descendants(ns + "UnitTestResult")
                                .Where(el => el.Attribute("outcome").Value.Equals("Passed") || el.Attribute("outcome").Value.Equals("Failed"))
                                .Select(x => new TestResult
                                {
                                    Name = x.Attribute("testName").Value,
                                    Result = x.Attribute("outcome").Value,
                                    DurationMs = (int)TimeSpan.Parse(x.Attribute("duration").Value).TotalMilliseconds,
                                    WorkflowRunJob = job
                                }).ToList();

            // add the tests to the job entity
            job.TestResults = results;

            return;
        }

        public bool CanProcessJob(WorkflowRunJob job)
        {
            return !string.IsNullOrEmpty(GetCategoryName(job));
        }

        private string GetCategoryName(WorkflowRunJob job)
        {
            // we get this through the job name
            var regEx = new Regex(@"API regression test \(([^,]*),");
            var matches = regEx.Matches(job.Name);
            if (matches.Any())
            {
                return matches[0].Groups[1].Value;
            }

            return string.Empty;
        }

        private string GetEnvironmentName(WorkflowRunJob job)
        {
            return job.Name.Split('/').FirstOrDefault()?.Trim();
        }
    }
}
