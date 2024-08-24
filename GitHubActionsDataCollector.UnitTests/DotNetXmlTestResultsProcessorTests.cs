using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Processors.JobProcessors;
using NSubstitute;
using System.IO.Compression;
using System.Text;

namespace GitHubActionsDataCollector.UnitTests
{
    public class DotNetXmlTestResultsProcessorTests
    {
        [Fact]
        public void CanProcessJob_ReturnsTrue_ForApiRegressionTestJob()
        {
            var client = NSubstitute.Substitute.For<IGitHubActionsApiClient>();

            var processor = new DotNetXmlTestResultsProcessor(client);

            Assert.True(processor.CanProcessJob(new GitHubActionsApi.WorkflowRunJobDto
            {
                name = "Regression1 migrate and test / Run smoke and regression tests / Regression Test / API regression test (Regression_Category_A, --filter \"Category=A\", 7)"
            }));
        }

        [Fact]
        public void CanProcessJob_ReturnsFalse_ForNonApiRegressionTestJob()
        {
            var client = NSubstitute.Substitute.For<IGitHubActionsApiClient>();

            var processor = new DotNetXmlTestResultsProcessor(client);

            Assert.False(processor.CanProcessJob(new GitHubActionsApi.WorkflowRunJobDto
            {
                name = "Regression1 migrate and test / Run smoke and regression tests / Smoke Test / Cypress Smoke Test (tests., 2)"
            }));
        }

        [Fact]
        public void CanProcessTestResultsXml()
        {
            var client = NSubstitute.Substitute.For<IGitHubActionsApiClient>();

            client.GetWorkflowRunArtifact(default, default, default, default).ReturnsForAnyArgs(s => GetZipArchiveStreamFromTestDoc());

            var processor = new DotNetXmlTestResultsProcessor(client);

            var jobDto = new WorkflowRunJobDto
            {
                name = "Regression1 migrate and test / Run smoke and regression tests / Regression Test / API regression test (Regression_Category_A, --filter \"Category=A\", 7)"
            };

            var artifactDto = new WorkflowRunArtifactsDto
            {
                artifacts = new List<WorkflowRunArtifactDto>
                {
                    new WorkflowRunArtifactDto
                    {
                        name = "Test Results Regression_Category_A",
                        id = 100
                    }
                }
            };

            var result = processor.Process("", "", "", jobDto, artifactDto);
        }

        private Stream GetZipArchiveStreamFromTestDoc()
        {
            var docString = @"<?xml version=""1.0"" encoding=""utf-8""?>
<TestRun id=""3d599961-73a4-4b83-83a2-d33d07a78c26"" name=""@6cffb57db91c 2024-08-15 23:23:33"" xmlns=""http://microsoft.com/schemas/VisualStudio/TeamTest/2010"">
  <Times creation=""2024-08-15T23:23:33.9296753+00:00"" queuing=""2024-08-15T23:23:33.9296754+00:00"" start=""2024-08-15T23:23:23.2267672+00:00"" finish=""2024-08-15T23:52:36.5534559+00:00"" />
  <TestSettings name=""default"" id=""cfca7808-87bd-475c-b184-b62ab4ec5c36"">
    <Deployment runDeploymentRoot=""_6cffb57db91c_2024-08-15_23_23_33"" />
  </TestSettings>
  <Results>
    <UnitTestResult executionId=""acfc84f6-b51a-44ee-82b4-4869e3f8907b"" testId=""080d2e5f-4600-50e4-e57e-28dbf67ad2c7"" testName=""Payroll.Tests.API.Manager.Au.AuManagerTimeAndAttendanceTests.TestManagerCanTriggerPinResetEmail"" computerName=""6cffb57db91c"" duration=""00:00:22.6809751"" startTime=""2024-08-15T23:44:12.7623032+00:00"" endTime=""2024-08-15T23:44:12.7623032+00:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""acfc84f6-b51a-44ee-82b4-4869e3f8907b"">
      <Output>
        <StdOut>Business ID: 1282761</StdOut>
      </Output>
    </UnitTestResult>
    <UnitTestResult executionId=""3f800dcf-a896-46cb-8430-d39752adaf50"" testId=""41a07f93-7605-8cbe-7fa3-875c97deb662"" testName=""Payroll.Tests.API.Business.Nz.MyPayCategoryTests.TestUpdatePayCategory"" computerName=""6cffb57db91c"" duration=""00:00:03.8983719"" startTime=""2024-08-15T23:49:14.7233942+00:00"" endTime=""2024-08-15T23:49:14.7233943+00:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""3f800dcf-a896-46cb-8430-d39752adaf50"">
      <Output>
        <StdOut>Business ID: 1282866</StdOut>
      </Output>
    </UnitTestResult>
    <UnitTestResult executionId=""4c8bd064-9c2f-46f2-ae34-ca2dd500334f"" testId=""0f4e2b2b-d1d2-3090-0d97-490ca08d56cb"" testName=""Payroll.Tests.API.BusinessEmployee.Au.AuStandardHoursTests.TestStandardHoursAdvanced1WeekReturnsWorkDaysInBusinessWeekOrder"" computerName=""6cffb57db91c"" duration=""00:00:15.9291574"" startTime=""2024-08-15T23:32:20.1877878+00:00"" endTime=""2024-08-15T23:32:20.1877878+00:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""4c8bd064-9c2f-46f2-ae34-ca2dd500334f"">
      <Output>
        <StdOut>Business ID: 1282549</StdOut>
      </Output>
    </UnitTestResult>
    <UnitTestResult executionId=""051b1547-ea22-4c0f-8c58-12130c3df70e"" testId=""b592c2fb-2bab-0857-686b-34111fad7355"" testName=""Payroll.Tests.API.Business.ShardedBusinessTimeAndAttendanceTests.ClockMeInClockOn_ShouldReturn200(businessId: 73525, kioskId: 2718, empId: 1210475, ???: 2)"" computerName=""6cffb57db91c"" duration=""00:00:00.0010000"" startTime=""2024-08-15T23:47:10.5629569+00:00"" endTime=""2024-08-15T23:47:10.5629570+00:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""NotExecuted"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""051b1547-ea22-4c0f-8c58-12130c3df70e"">
      <Output>
        <ErrorInfo>
          <Message>Test skipped for non-prod environments</Message>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
    <UnitTestResult executionId=""44cb76bc-41f4-4fff-8afb-47c1d8fd167a"" testId=""75f1300e-6439-a505-72b0-b5c3f8b646af"" testName=""Payroll.Tests.API.Business.Au.AuEmployeeExpenseCategoryTests.TestCreateAndGetExpenseCategory"" computerName=""6cffb57db91c"" duration=""00:00:01.3327195"" startTime=""2024-08-15T23:50:37.5685692+00:00"" endTime=""2024-08-15T23:50:37.5685693+00:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""44cb76bc-41f4-4fff-8afb-47c1d8fd167a"">
      <Output>
        <StdOut>Business ID: 1282893</StdOut>
      </Output>
    </UnitTestResult>
    <UnitTestResult executionId=""95c83dac-0f77-447d-87b7-aa3e219e0f7d"" testId=""ca371e0f-9bd7-e474-17c0-55f7defc7a4e"" testName=""Payroll.Tests.API.Business.AuJournalAccountsTests.TestCreateJournalAccount"" computerName=""6cffb57db91c"" duration=""00:00:00.6670170"" startTime=""2024-08-15T23:43:18.0040051+00:00"" endTime=""2024-08-15T23:43:18.0040051+00:00"" testType=""13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b"" outcome=""Passed"" testListId=""8c84fa94-04c1-424b-9868-57a2d4851a1d"" relativeResultsDirectory=""95c83dac-0f77-447d-87b7-aa3e219e0f7d"">
      <Output>
        <StdOut>Business ID: 1282755</StdOut>
      </Output>
    </UnitTestResult>
</Results>
</TestRun>";

            byte[] byteArray = Encoding.UTF8.GetBytes(docString);
            var ms = new MemoryStream(byteArray);

            var outStream = new MemoryStream();
            using (var archive = new ZipArchive(outStream, ZipArchiveMode.Create, true))
            {
                var entry = archive.CreateEntry("test.xml");
                using (var entryStream = entry.Open())
                {
                    ms.CopyTo(entryStream);
                }
            }

            return outStream;
        }
    }
}
