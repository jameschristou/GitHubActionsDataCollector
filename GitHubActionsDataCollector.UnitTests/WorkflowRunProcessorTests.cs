using GitHubActionsDataCollector.Entities;
using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Processors;
using GitHubActionsDataCollector.Repositories;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.UnitTests
{
    public class WorkflowRunProcessorTests
    {
        [Fact]
        public async Task WorkflowRunHasNoConclusion_ShouldNotBeSaved()
        {
            var workflowRunJobsProcessor = Substitute.For<IWorkflowRunJobsProcessor>();
            var workflowRunRepository = Substitute.For<IWorkflowRunRepository>();

            var registeredWorkflowSettings = new WorkflowRunSettings();

            var registeredWorkflow = new RegisteredWorkflow()
            {
                Settings = JsonSerializer.Serialize(registeredWorkflowSettings)
            };

            var workflowRunDto = new WorkflowRunDto
            {
                conclusion = "",
                created_at = "2024/09/07",
                updated_at = "2024/09/07"
            };

            var workflowRunProcessor = new WorkflowRunProcessor(workflowRunJobsProcessor, workflowRunRepository);

            await workflowRunProcessor.Process(registeredWorkflow, workflowRunDto);

            // check that the run is not saved
            await workflowRunRepository.DidNotReceive().SaveWorkflowRun(Arg.Any<WorkflowRun>());
        }

        [Fact]
        public async Task WorkflowRunIsMarkedAsSuccess_WhenConclusionIsSuccessAndThereAreJobsToProcess()
        {
            var workflowRunJobsProcessor = Substitute.For<IWorkflowRunJobsProcessor>();
            workflowRunJobsProcessor.Process(default, default, default, default, default).ReturnsForAnyArgs(x => new List<WorkflowRunJob> { new WorkflowRunJob { Conclusion = "success" } });

            var workflowRunRepository = Substitute.For<IWorkflowRunRepository>();

            var registeredWorkflowSettings = new WorkflowRunSettings();

            var registeredWorkflow = new RegisteredWorkflow()
            {
                Settings = JsonSerializer.Serialize(registeredWorkflowSettings)
            };

            var workflowRunDto = new WorkflowRunDto
            {
                conclusion = "success",
                created_at = "2024/09/07",
                updated_at = "2024/09/07"
            };

            var workflowRunProcessor = new WorkflowRunProcessor(workflowRunJobsProcessor, workflowRunRepository);

            await workflowRunProcessor.Process(registeredWorkflow, workflowRunDto);

            // check that the run is saved with conclusion success
            await workflowRunRepository.Received().SaveWorkflowRun(Arg.Is<WorkflowRun>(x => x.Conclusion == "success"));
        }

        [Fact]
        public async Task WorkflowRunIsMarkedAsSuccess_WhenConfiguredJobNameSucceedsAndRunFails()
        {
            var jobNameForSuccess = "Job Name for Success";

            var workflowRunJobsProcessor = Substitute.For<IWorkflowRunJobsProcessor>();
            var workflowRunRepository = Substitute.For<IWorkflowRunRepository>();
            workflowRunJobsProcessor.Process(default, default, default, default, default).ReturnsForAnyArgs(x => new List<WorkflowRunJob> { new WorkflowRunJob { Name = jobNameForSuccess, Conclusion = "success" } });

            var registeredWorkflowSettings = new WorkflowRunSettings()
            {
                JobNameRequiredForRunSuccess = jobNameForSuccess
            };

            var registeredWorkflow = new RegisteredWorkflow()
            {
                Settings = JsonSerializer.Serialize(registeredWorkflowSettings)
            };

            var workflowRunDto = new WorkflowRunDto
            {
                conclusion = "failure",
                created_at = "2024/09/07",
                updated_at = "2024/09/07"
            };

            var workflowRunProcessor = new WorkflowRunProcessor(workflowRunJobsProcessor, workflowRunRepository);

            await workflowRunProcessor.Process(registeredWorkflow, workflowRunDto);

            // check that the run is saved with conclusion success
            await workflowRunRepository.Received().SaveWorkflowRun(Arg.Is<WorkflowRun>(x => x.Conclusion == "success"));
        }

        [Fact]
        public async Task WorkflowRunIsMarkedAsFailure_WhenNoJobsReturn()
        {
            var workflowRunJobsProcessor = Substitute.For<IWorkflowRunJobsProcessor>();
            var workflowRunRepository = Substitute.For<IWorkflowRunRepository>();
            workflowRunJobsProcessor.Process(default, default, default, default, default).ReturnsForAnyArgs(x => new List<WorkflowRunJob>());

            var registeredWorkflowSettings = new WorkflowRunSettings()
            {
                JobNameRequiredForRunSuccess = "Prod / Post Deploy Tasks"
            };

            var registeredWorkflow = new RegisteredWorkflow()
            {
                Settings = JsonSerializer.Serialize(registeredWorkflowSettings)
            };

            var workflowRunDto = new WorkflowRunDto
            {
                conclusion = "failure",
                created_at = "2024/09/07",
                updated_at = "2024/09/07"
            };

            var workflowRunProcessor = new WorkflowRunProcessor(workflowRunJobsProcessor, workflowRunRepository);

            await workflowRunProcessor.Process(registeredWorkflow, workflowRunDto);

            // check that the run is not saved
            await workflowRunRepository.DidNotReceive().SaveWorkflowRun(Arg.Any<WorkflowRun>());
        }
    }
}
