using GitHubActionsDataCollector.GitHubActionsApi;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.Processors.JobProcessors
{
    public class DotNetXmlTestResultsProcessor : IJobProcessor
    {
        public async Task Process(WorkflowRunJobDto job, WorkflowRunArtifactsDto artifacts)
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

            // now we download the artifact

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
