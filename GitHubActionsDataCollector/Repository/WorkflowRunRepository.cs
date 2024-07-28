using GitHubActionsDataCollector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.Repository
{
    public interface IWorkflowRunRepository
    {
        public void SaveWorkflowRun(WorkflowRunModel workflowRun);
    }

    internal class WorkflowRunRepository : IWorkflowRunRepository
    {
        public void SaveWorkflowRun(WorkflowRunModel workflowRun)
        {
            Console.WriteLine($"Saving workflowRun:{workflowRun.Id}");
        }
    }
}
