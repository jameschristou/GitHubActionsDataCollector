using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector
{
    internal class Processor
    {
        private readonly IWorkflowRunProcessor _workflowRunProcessor;

        public Processor(IWorkflowRunProcessor workflowRunProcessor)
        {
            _workflowRunProcessor = workflowRunProcessor;
        }

        public void Run()
        { 
            // first get the list of workflow runs from the API client

            // then process each workflow run
        }
    }
}
