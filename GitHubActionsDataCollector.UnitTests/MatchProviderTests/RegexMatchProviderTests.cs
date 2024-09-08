using GitHubActionsDataCollector.Processors.JobProcessors;

namespace GitHubActionsDataCollector.UnitTests.MatchProviderTests
{
    public class RegexMatchProviderTests
    {
        [Fact]
        public void IsMatch()
        {
            var matchProvider = new RegExMatchProvider();

            Assert.True(matchProvider.IsMatch("API regression tests Group 1", "API (regression|smoke) tests"));
        }

        [Fact]
        public void IsNotMatch()
        {
            var matchProvider = new RegExMatchProvider();

            Assert.False(matchProvider.IsMatch("cypress regression tests Group 1", "API (regression|smoke) tests"));
        }
    }
}
