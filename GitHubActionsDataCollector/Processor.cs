﻿using GitHubActionsDataCollector.Processors;
using GitHubActionsDataCollector.Repositories;

namespace GitHubActionsDataCollector
{
    public class Processor
    {
        private readonly IRegisteredWorkflowRepository _registeredWorkflowRepository;
        private readonly IRegisteredWorkflowProcessor _registeredWorkflowProcessor;

        private const int MaxRegisteredWorkflowsToProcess = 2;

        public Processor(IRegisteredWorkflowRepository registeredWorkflowRepository,
                            IRegisteredWorkflowProcessor registeredWorkflowProcessor)
        {
            _registeredWorkflowRepository = registeredWorkflowRepository;
            _registeredWorkflowProcessor = registeredWorkflowProcessor;
        }

        public async Task Run()
        {
            var workflow = await _registeredWorkflowRepository.GetLeastRecentlyCheckedWorkflow();

            if (workflow == null)
            {
                Console.WriteLine("No workflows to process!");
                return;
            }

            await _registeredWorkflowProcessor.Process(workflow);
        }
    }
}