﻿using System.Globalization;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text.Json;

namespace GitHubActionsDataCollector.GitHubActionsApiClient
{
    public interface IGitHubActionsApiClient
    {
        Task<WorkflowRunListDto> GetWorkflowRuns(string repoOwner, string repoName, string token, long workflowId, DateTime fromDate, DateTime toDate, int pageNumber, int resultsPerPage);
        Task<WorkflowRunJobsListDto> GetJobsForWorkflowRun(string repoOwner, string repoName, string token, long workflowRunId, int pageNumber, int resultsPerPage);
    }

    internal class GitHubActionsApiClient : IGitHubActionsApiClient
    {
        private static string baseUrl = "https://api.github.com";
        private readonly HttpClient _httpClient;

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

            var response = await _httpClient.SendAsync(BuildRequest(url, token));

            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to retrieve workflow runs for workflow:{workflowId}");
            }

            return JsonSerializer.Deserialize<WorkflowRunListDto>(await response.Content.ReadAsStringAsync());
        }

        private WorkflowRunListDto GetWorkflowRunsMocked(string repoOwner, string repoName, long workflowId, DateTime fromDate, int pageNumber, int resultsPerPage)
        {
            // [Get workflow runs for a workflow](https://docs.github.com/en/rest/actions/workflow-runs?apiVersion=2022-11-28#list-workflow-runs-for-a-workflow)
            var resultString = @"
                {
    ""total_count"": 1638,
    ""workflow_runs"": [
        {
            ""id"": 10111150555,
            ""name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""display_title"": ""Merge pull request #12153 from OwnerName/someone/deploy-26-jul-2024"",
            ""event"": ""push"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""workflow_id"": 12345,
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/12345"",
            ""pull_requests"": [],
            ""created_at"": ""2024-07-26T12:21:48Z"",
            ""updated_at"": ""2024-07-26T14:58:31Z"",
            ""actor"": {
                ""login"": ""JaneDoe"",
                ""id"": 12345,
                ""avatar_url"": ""https://avatars.githubusercontent.com/u/6419944?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/JaneDoe"",
                ""html_url"": ""https://github.com/JaneDoe"",
                ""followers_url"": ""https://api.github.com/users/JaneDoe/followers"",
                ""following_url"": ""https://api.github.com/users/JaneDoe/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/JaneDoe/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/JaneDoe/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/JaneDoe/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/JaneDoe/orgs"",
                ""repos_url"": ""https://api.github.com/users/JaneDoe/repos"",
                ""events_url"": ""https://api.github.com/users/JaneDoe/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/JaneDoe/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
            },
            ""run_attempt"": 3,
            ""run_started_at"": ""2024-07-26T14:35:00Z"",
            ""triggering_actor"": {
                ""login"": ""JaneDoe"",
                ""id"": 6419944,
                ""avatar_url"": ""https://avatars.githubusercontent.com/u/6419944?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/JaneDoe"",
                ""html_url"": ""https://github.com/JaneDoe"",
                ""followers_url"": ""https://api.github.com/users/JaneDoe/followers"",
                ""following_url"": ""https://api.github.com/users/JaneDoe/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/JaneDoe/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/JaneDoe/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/JaneDoe/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/JaneDoe/orgs"",
                ""repos_url"": ""https://api.github.com/users/JaneDoe/repos"",
                ""events_url"": ""https://api.github.com/users/JaneDoe/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/JaneDoe/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
            },
            ""jobs_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/jobs"",
            ""logs_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/logs"",
            ""check_suite_url"": ""https://api.github.com/repos/OwnerName/repo-name/check-suites/12345"",
            ""artifacts_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/artifacts"",
            ""cancel_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/cancel"",
            ""rerun_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/rerun"",
            ""previous_attempt_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/attempts/2"",
            ""workflow_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/workflows/12345""
        },
        {
            ""id"": 10111128321,
            ""name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""display_title"": ""Merge pull request #12174 from OwnerName/blah/fix_something"",
            ""run_number"": 1703,
            ""event"": ""push"",
            ""status"": ""completed"",
            ""conclusion"": ""cancelled"",
            ""workflow_id"": 12345,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/12345"",
            ""pull_requests"": [],
            ""created_at"": ""2024-07-26T12:20:11Z"",
            ""updated_at"": ""2024-07-26T12:22:09Z"",
            ""actor"": {
                ""login"": ""JaneDoe"",
                ""id"": 12345,
                ""avatar_url"": ""https://avatars.githubusercontent.com/u/6419944?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/JaneDoe"",
                ""html_url"": ""https://github.com/JaneDoe"",
                ""followers_url"": ""https://api.github.com/users/JaneDoe/followers"",
                ""following_url"": ""https://api.github.com/users/JaneDoe/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/JaneDoe/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/JaneDoe/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/JaneDoe/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/JaneDoe/orgs"",
                ""repos_url"": ""https://api.github.com/users/JaneDoe/repos"",
                ""events_url"": ""https://api.github.com/users/JaneDoe/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/JaneDoe/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
            },
            ""run_attempt"": 1,
            ""run_started_at"": ""2024-07-26T12:20:11Z"",
            ""triggering_actor"": {
                ""login"": ""JaneDoe"",
                ""id"": 12345,
                ""avatar_url"": ""https://avatars.githubusercontent.com/u/6419944?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/JaneDoe"",
                ""html_url"": ""https://github.com/JaneDoe"",
                ""followers_url"": ""https://api.github.com/users/JaneDoe/followers"",
                ""following_url"": ""https://api.github.com/users/JaneDoe/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/JaneDoe/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/JaneDoe/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/JaneDoe/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/JaneDoe/orgs"",
                ""repos_url"": ""https://api.github.com/users/JaneDoe/repos"",
                ""events_url"": ""https://api.github.com/users/JaneDoe/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/JaneDoe/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
            },
            ""jobs_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/jobs"",
            ""logs_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/logs"",
            ""check_suite_url"": ""https://api.github.com/repos/OwnerName/repo-name/check-suites/12345"",
            ""artifacts_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/artifacts"",
            ""cancel_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/cancel"",
            ""rerun_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/rerun"",
            ""previous_attempt_url"": null,
            ""workflow_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/workflows/12345""
        },
        {
            ""id"": 10106100121,
            ""name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""display_title"": ""Merge pull request #12166 from OwnerName/blap/fix-another-thing"",
            ""run_number"": 1702,
            ""event"": ""push"",
            ""status"": ""completed"",
            ""conclusion"": ""cancelled"",
            ""workflow_id"": 12345,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/12345"",
            ""pull_requests"": [],
            ""created_at"": ""2024-07-26T05:14:47Z"",
            ""updated_at"": ""2024-07-26T13:47:15Z"",
            ""actor"": {
                ""login"": ""JohnSnow"",
                ""id"": 12345,
                ""avatar_url"": ""https://avatars.githubusercontent.com/u/112647275?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/JohnSnow"",
                ""html_url"": ""https://github.com/JohnSnow"",
                ""followers_url"": ""https://api.github.com/users/JohnSnow/followers"",
                ""following_url"": ""https://api.github.com/users/JohnSnow/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/JohnSnow/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/JohnSnow/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/JohnSnow/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/JohnSnow/orgs"",
                ""repos_url"": ""https://api.github.com/users/JohnSnow/repos"",
                ""events_url"": ""https://api.github.com/users/JohnSnow/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/JohnSnow/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
            },
            ""run_attempt"": 2,
            ""run_started_at"": ""2024-07-26T06:30:31Z"",
            ""triggering_actor"": {
                ""login"": ""JohnSnow"",
                ""id"": 12345,
                ""avatar_url"": ""https://avatars.githubusercontent.com/u/112647275?v=4"",
                ""gravatar_id"": """",
                ""url"": ""https://api.github.com/users/JohnSnow"",
                ""html_url"": ""https://github.com/JohnSnow"",
                ""followers_url"": ""https://api.github.com/users/JohnSnow/followers"",
                ""following_url"": ""https://api.github.com/users/JohnSnow/following{/other_user}"",
                ""gists_url"": ""https://api.github.com/users/JohnSnow/gists{/gist_id}"",
                ""starred_url"": ""https://api.github.com/users/JohnSnow/starred{/owner}{/repo}"",
                ""subscriptions_url"": ""https://api.github.com/users/JohnSnow/subscriptions"",
                ""organizations_url"": ""https://api.github.com/users/JohnSnow/orgs"",
                ""repos_url"": ""https://api.github.com/users/JohnSnow/repos"",
                ""events_url"": ""https://api.github.com/users/JohnSnow/events{/privacy}"",
                ""received_events_url"": ""https://api.github.com/users/JohnSnow/received_events"",
                ""type"": ""User"",
                ""site_admin"": false
            },
            ""jobs_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/jobs"",
            ""logs_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/logs"",
            ""check_suite_url"": ""https://api.github.com/repos/OwnerName/repo-name/check-suites/12345"",
            ""artifacts_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/artifacts"",
            ""cancel_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/cancel"",
            ""rerun_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/rerun"",
            ""previous_attempt_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345/attempts/1"",
            ""workflow_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/workflows/12345""
        }
    ]
}
            ";

            return JsonSerializer.Deserialize<WorkflowRunListDto>(resultString);
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

            var response = await _httpClient.SendAsync(BuildRequest(url, token));

            if (response == null || !response.IsSuccessStatusCode)
            {
                throw new Exception($"Unable to retrieve workflow run jobs for workflowRun:{workflowRunId}");
            }

            return JsonSerializer.Deserialize<WorkflowRunJobsListDto>(await response.Content.ReadAsStringAsync());

            //return pageNumber == 1 ? GetJobsForWorkflowRunPage1() : GetJobsForWorkflowRunPage2();
        }

        private WorkflowRunJobsListDto GetJobsForWorkflowRunPage1()
        {
            var resultString = @"
                {
    ""total_count"": 6,
    ""jobs"": [
        {
            ""id"": 1234,
            ""run_id"": 123456789,
            ""workflow_name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""run_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/123456789"",
            ""run_attempt"": 1,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/jobs/1234"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/123456789/job/1234"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""created_at"": ""2024-07-24T22:31:48Z"",
            ""started_at"": ""2024-07-24T22:32:53Z"",
            ""completed_at"": ""2024-07-24T22:46:29Z"",
            ""name"": ""Build / Build NetFX""
        },
        {
            ""id"": 12345,
            ""run_id"": 123456789,
            ""workflow_name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""run_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/12345"",
            ""run_attempt"": 1,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/jobs/27884800125"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/123456789/job/12345"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""created_at"": ""2024-07-24T22:31:48Z"",
            ""started_at"": ""2024-07-24T22:32:05Z"",
            ""completed_at"": ""2024-07-24T22:41:44Z"",
            ""name"": ""Build / Build/Test Netcore""
        },
        {
            ""id"": 123456,
            ""run_id"": 123456789,
            ""workflow_name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""run_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/123456789"",
            ""run_attempt"": 1,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/jobs/27885205291"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/123456789/job/27885205291"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""created_at"": ""2024-07-24T22:46:30Z"",
            ""started_at"": ""2024-07-24T22:46:39Z"",
            ""completed_at"": ""2024-07-24T23:01:00Z"",
            ""name"": ""Build / Test NetFX""
        },
        {
            ""id"": 1234567,
            ""run_id"": 123456789,
            ""workflow_name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""run_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/123456789"",
            ""run_attempt"": 1,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/jobs/1234567"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/123456789/job/1234567"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""created_at"": ""2024-07-24T22:46:31Z"",
            ""started_at"": ""2024-07-24T22:46:36Z"",
            ""completed_at"": ""2024-07-24T22:53:34Z"",
            ""name"": ""Build / Build front-end""
        }
    ]
}
            ";

            return JsonSerializer.Deserialize<WorkflowRunJobsListDto>(resultString);
        }

        private WorkflowRunJobsListDto GetJobsForWorkflowRunPage2()
        {
            var resultString = @"
                {
    ""total_count"": 6,
    ""jobs"": [
        {
            ""id"": 12345678,
            ""run_id"": 123456789,
            ""workflow_name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""run_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/123456789"",
            ""run_attempt"": 1,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/jobs/12345678"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/123456789/job/12345678"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""created_at"": ""2024-07-24T22:31:48Z"",
            ""started_at"": ""2024-07-24T22:32:53Z"",
            ""completed_at"": ""2024-07-24T22:46:29Z"",
            ""name"": ""Run more tests"",
            ""steps"": [
                {
                    ""name"": ""Set up job"",
                    ""status"": ""completed"",
                    ""conclusion"": ""success"",
                    ""number"": 1,
                    ""started_at"": ""2024-07-24T23:33:51Z"",
                    ""completed_at"": ""2024-07-24T23:33:52Z""
                },
                {
                    ""name"": ""Run tests"",
                    ""status"": ""completed"",
                    ""conclusion"": ""success"",
                    ""number"": 2,
                    ""started_at"": ""2024-07-24T23:34:29Z"",
                    ""completed_at"": ""2024-07-25T00:24:35Z""
                },
                {
                    ""name"": ""Complete job"",
                    ""status"": ""completed"",
                    ""conclusion"": ""success"",
                    ""number"": 3,
                    ""started_at"": ""2024-07-25T00:24:36Z"",
                    ""completed_at"": ""2024-07-25T00:24:37Z""
                }
            ]
        },
        {
            ""id"": 123456789,
            ""run_id"": 123456789,
            ""workflow_name"": ""* Staging Build"",
            ""head_branch"": ""staging"",
            ""run_url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/runs/123456789"",
            ""run_attempt"": 1,
            ""url"": ""https://api.github.com/repos/OwnerName/repo-name/actions/jobs/27884800125"",
            ""html_url"": ""https://github.com/OwnerName/repo-name/actions/runs/123456789/job/123456789"",
            ""status"": ""completed"",
            ""conclusion"": ""success"",
            ""created_at"": ""2024-07-24T23:31:48Z"",
            ""started_at"": ""2024-07-24T22:32:05Z"",
            ""completed_at"": ""2024-07-24T22:41:44Z"",
            ""name"": ""Build / Build/Test Netcore""
        }
    ]
}
            ";

            return JsonSerializer.Deserialize<WorkflowRunJobsListDto>(resultString);
        }
    }
}
