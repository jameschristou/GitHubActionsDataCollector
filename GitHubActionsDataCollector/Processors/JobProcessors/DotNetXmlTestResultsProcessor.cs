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

        public async Task Process(string owner, string repo, string token, WorkflowRunJobDto job, WorkflowRunArtifactsDto artifacts)
        {
            var categoryName = GetCategoryName(job);

            if(string.IsNullOrEmpty(categoryName))
            {
                Console.WriteLine($"Could not get category name for job:{job.name}");
                return;
            }

            // first we need to get the artifact file for this job
            var artifact = artifacts.artifacts.FirstOrDefault(a => a.name.Contains(categoryName, StringComparison.OrdinalIgnoreCase));

            if(artifact == null)
            {
                Console.WriteLine($"Could not get artifact name for job:{job.name}");
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

            var results = xdoc.Descendants(ns + "UnitTestResult").ToList();

            // then we extract the file

            // load it as an XML document

            // extract all the test results

            // process each result one by one

            // need a way to save the test results

            return;
        }

        public bool CanProcessJob(WorkflowRunJobDto job)
        {
            return !string.IsNullOrEmpty(GetCategoryName(job));
        }

        private string GetCategoryName(WorkflowRunJobDto job)
        {
            // we get this through the job name
            var regEx = new Regex(@"API regression test \(([^,]*),");
            var matches = regEx.Matches(job.name);
            if (matches.Any())
            {
                return matches[0].Groups[1].Value;
            }

            return string.Empty;
        }
    }
}
