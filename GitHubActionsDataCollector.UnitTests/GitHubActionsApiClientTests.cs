using System.Net;
using GitHubActionsDataCollector.GitHubActionsApi;

namespace GitHubActionsDataCollector.UnitTests
{
    public class GitHubActionsApiClientTests
    {
        [Fact]
        public async Task TestGetWorkflowRuns()
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

            var mockHttpMessageHandler = new MockHttpMessageHandler(resultString, HttpStatusCode.OK);

            var httpClient = new HttpClient(mockHttpMessageHandler);

            var gitHubActionsApiClient = new GitHubActionsApiClient(httpClient);

            var response = await gitHubActionsApiClient.GetWorkflowRuns("", "", "", 1234, new DateTime(2020, 01, 01), new DateTime(2020, 02, 01), 10, 10);

            Assert.Equal(3, response.workflow_runs.Count());
        }
    }

    public class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public MockHttpMessageHandler(string response, HttpStatusCode statusCode)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response)
            };
        }
    }
}