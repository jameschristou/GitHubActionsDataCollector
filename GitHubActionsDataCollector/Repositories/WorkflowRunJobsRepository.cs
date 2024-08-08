using GitHubActionsDataCollector.Entities;
using NHibernate;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IWorkflowRunJobsRepository
    {
        public void SaveJobs(List<WorkflowRunJob> jobs);
    }
    public class WorkflowRunJobsRepository : IWorkflowRunJobsRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public WorkflowRunJobsRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void SaveJobs(List<WorkflowRunJob> jobs)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    jobs.ForEach(x => session.Save(x));
                    transaction.Commit();
                }
            }

            Console.WriteLine("Saving workflowRun jobs");
        }
    }
}
