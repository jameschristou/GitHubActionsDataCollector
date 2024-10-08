﻿using NHibernate.Mapping;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace GitHubActionsDataCollector.GitHubActionsApi
{
    public interface IGitHubActionsApiClient
    {
        Task<WorkflowRunListDto> GetWorkflowRuns(string repoOwner, string repoName, string token, long workflowId, DateTime fromDate, DateTime toDate, int pageNumber, int resultsPerPage);
        Task<WorkflowRunJobsListDto> GetJobsForWorkflowRun(string repoOwner, string repoName, string token, long workflowRunId, int pageNumber, int resultsPerPage);
        Task<WorkflowRunArtifactsDto> GetArtifactsListforWorkflowRun(string owner, string repo, string token, long workflowRunId);
        Task<Stream> GetWorkflowRunArtifact(string owner, string repo, string token, long artifactId);
        Task<Stream> GetWorkflowRunAttemptLogs(string owner, string repo, string token, long workflowRunId, int attemptNumber);
    }

    public class GitHubActionsApiClient : IGitHubActionsApiClient
    {
        private static string baseUrl = "https://api.github.com";
        private readonly HttpClient _httpClient;
        private readonly int RateLimitMaxPercentageUsed = 50; // 50%

        public GitHubActionsApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Gets paged workflow runs for a given workflow
        /// </summary>
        public async Task<WorkflowRunListDto> GetWorkflowRuns(string owner, string repo, string token, long workflowId, DateTime fromDate, DateTime toDate, 
                                                            int pageNumber, int resultsPerPage)
        {
            var fromDateFormatted = fromDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            var toDateFormatted = toDate.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);

            // [Get workflow runs for a workflow](https://docs.github.com/en/rest/actions/workflow-runs?apiVersion=2022-11-28#list-workflow-runs-for-a-workflow)
            var url = $"{baseUrl}/repos/{owner}/{repo}/actions/workflows/{workflowId}/runs?per_page={resultsPerPage}&page={pageNumber}&created={fromDateFormatted}..{toDateFormatted}";

            var response = await SendRequestAsync(url, token);

            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to retrieve workflow runs for workflow:{workflowId}");
            }

            return JsonSerializer.Deserialize<WorkflowRunListDto>(await response.Content.ReadAsStringAsync());
        }

        private HttpRequestMessage BuildRequest(string url, string token)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            // create a personal access token with github. See instructions here
            // https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-personal-access-token-classic

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            requestMessage.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            requestMessage.Headers.Add("User-agent", "request");

            if (!string.IsNullOrEmpty(token))
            {
                requestMessage.Headers.Add("Authorization", $"Bearer {token}");
            }

            return requestMessage;
        }

        /// <summary>
        /// Gets paged workflow run jobs for a given workflow run
        /// </summary>
        public async Task<WorkflowRunJobsListDto> GetJobsForWorkflowRun(string owner, string repo, string token, long workflowRunId, int pageNumber, int resultsPerPage)
        {
            // [List jobs for a workflow run](https://docs.github.com/en/rest/actions/workflow-jobs?apiVersion=2022-11-28#list-jobs-for-a-workflow-run)
            var url = $"{baseUrl}/repos/{owner}/{repo}/actions/runs/{workflowRunId}/jobs?per_page={resultsPerPage}&page={pageNumber}&filter=all";

            var response = await SendRequestAsync(url, token);

            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to retrieve workflow run jobs for workflowRun:{workflowRunId}");
            }

            return JsonSerializer.Deserialize<WorkflowRunJobsListDto>(await response.Content.ReadAsStringAsync());
        }

        public async Task<WorkflowRunArtifactsDto> GetArtifactsListforWorkflowRun(string owner, string repo, string token, long workflowRunId)
        {
            // TODO: implement paging. For now we assume that we will never need more than 100 artifacts
            // [List artifacts for a workflow run](// [List jobs for a workflow run](https://docs.github.com/en/rest/actions/workflow-jobs?apiVersion=2022-11-28#list-jobs-for-a-workflow-run)
            var url = $"{baseUrl}/repos/{owner}/{repo}/actions/runs/{workflowRunId}/artifacts?per_page=100&page=1";

            var response = await SendRequestAsync(url, token);

            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to retrieve workflow run artifacts for workflowRun:{workflowRunId}");
            }

            return JsonSerializer.Deserialize<WorkflowRunArtifactsDto>(await response.Content.ReadAsStringAsync());
        }

        public async Task<Stream> GetWorkflowRunArtifact(string owner, string repo, string token, long artifactId)
        {
            // [Get download URL for an artifact](// [Download an artifact](https://docs.github.com/en/rest/actions/artifacts?apiVersion=2022-11-28#download-an-artifact)
            var url = $"{baseUrl}/repos/{owner}/{repo}/actions/artifacts/{artifactId}/zip";

            var response = await SendRequestAsync(url, token);

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to retrieve workflow run artifact:{artifactId}");
            }

            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<Stream> GetWorkflowRunAttemptLogs(string owner, string repo, string token, long workflowRunId, int attemptNumber)
        {
            // [Download run attempt logs](https://docs.github.com/en/rest/actions/workflow-runs?apiVersion=2022-11-28#download-workflow-run-attempt-logs)
            var url = $"{baseUrl}/repos/{owner}/{repo}/actions/runs/{workflowRunId}/attempts/{attemptNumber}/logs";

            var response = await SendRequestAsync(url, token);

            if (response == null || response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"Unable to retrieve workflow run attempt logs:{workflowRunId} attemptNumber:{attemptNumber}");
            }

            return await response.Content.ReadAsStreamAsync();
        }

        private async Task<HttpResponseMessage> SendRequestAsync(string url, string token)
        {
            var response = await _httpClient.SendAsync(BuildRequest(url, token));

            // validate that we are not exceeding the rate limit cap...however not all requests return
            // the headers required so we can't assume these can be checked on every request
            if (response == null) return null;

            IEnumerable<string> values;
            var rateLimitRemaining = 0;
            var rateLimit = 0;

            if (response.Headers.TryGetValues("x-ratelimit-limit", out values))
            {
                if (!int.TryParse(values.First(), out rateLimit))
                {
                    throw new Exception("Unable to check rate limit remaining");
                }
            }

            if (response.Headers.TryGetValues("x-ratelimit-remaining", out values))
            {
                if (!int.TryParse(values.First(), out rateLimitRemaining))
                {
                    throw new Exception("Unable to check rate limit remaining");
                }
            }

            if (rateLimitRemaining != 0 && rateLimit != 0 && ((rateLimitRemaining*100/rateLimit) < RateLimitMaxPercentageUsed))
            {
                throw new Exception("Rate limit cap exceeded. Cannot process any more requests until the current window resets");
            }

            return response;
        }
    }
}
