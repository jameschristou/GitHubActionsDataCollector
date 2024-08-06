using GitHubActionsDataCollector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IWorkflowRunJobsRepository
    {
        public void SaveJobs(List<WorkflowRunJobModel> jobs);
    }
    public class WorkflowRunJobsRepository : IWorkflowRunJobsRepository
    {
        public void SaveJobs(List<WorkflowRunJobModel> jobs)
        {
            
        }
    }
}
