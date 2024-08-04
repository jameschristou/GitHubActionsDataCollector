using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.GitHubActionsApiClient
{
    public interface IGitHubActionsApiClient
    {
        WorkflowRunListDto GetWorkflowRuns(string repoOwner, string repoName, long workflowId, DateTime fromDate, int pageNumber, int resultsPerPage);
        Task<WorkflowRunJobsListDto> GetJobsForWorkflowRun(string repoOwner, string repoName, long workflowRunId, int pageNumber, int resultsPerPage);
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
        public WorkflowRunListDto GetWorkflowRuns(string repoOwner, string repoName, long workflowId, DateTime fromDate, int pageNumber, int resultsPerPage)
        {
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

        /// <summary>
        /// Gets paged workflow run jobs for a given workflow run
        /// </summary>
        public async Task<WorkflowRunJobsListDto> GetJobsForWorkflowRun(string repoOwner, string repoName, long workflowRunId, int pageNumber, int resultsPerPage)
        {
            //workflowRunId = 0;
            //repoOwner = "";
            //repoName = "";

            var url = $"{baseUrl}/repos/{repoOwner}/{repoName}/actions/runs/{workflowRunId}/jobs?per_page={resultsPerPage}&page={pageNumber}&filter=all";

            var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            // create a personal access token with github. See instructions here
            // https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-personal-access-token-classic
            var token = "";

            requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            requestMessage.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            requestMessage.Headers.Add("Authorization", $"Bearer {token}");

            //var response = await _httpClient.SendAsync(requestMessage);

            //if (response == null || !response.IsSuccessStatusCode)
            //{
            //    throw new Exception("Unable to retrieve workflow run jobs");
            //}

            return pageNumber == 1 ? GetJobsForWorkflowRunPage1() : GetJobsForWorkflowRunPage2();
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
            ""name"": ""Run more tests""
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
