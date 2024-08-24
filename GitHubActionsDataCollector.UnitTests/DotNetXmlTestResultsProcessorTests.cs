using GitHubActionsDataCollector.Processors.JobProcessors;

namespace GitHubActionsDataCollector.UnitTests
{
    public class DotNetXmlTestResultsProcessorTests
    {
        [Fact]
        public void CanProcessJob_ReturnsTrue_ForApiRegressionTestJob()
        {
            var processor = new DotNetXmlTestResultsProcessor();

            Assert.True(processor.CanProcessJob(new GitHubActionsApi.WorkflowRunJobDto
            {
                name = "Regression1 migrate and test / Run smoke and regression tests / Regression Test / API regression test (Regression_Category_A, --filter \"Category=A\", 7)"
            }));
        }

        [Fact]
        public void CanProcessJob_ReturnsFalse_ForNonApiRegressionTestJob()
        {
            var processor = new DotNetXmlTestResultsProcessor();

            Assert.False(processor.CanProcessJob(new GitHubActionsApi.WorkflowRunJobDto
            {
                name = "Regression1 migrate and test / Run smoke and regression tests / Smoke Test / Cypress Smoke Test (tests., 2)"
            }));
        }
    }
}
