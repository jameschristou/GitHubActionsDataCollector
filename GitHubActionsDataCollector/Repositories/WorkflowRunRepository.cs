using GitHubActionsDataCollector.Entities;
using NHibernate;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IWorkflowRunRepository
    {
        public void SaveWorkflowRun(WorkflowRun workflowRun);
    }

    public class WorkflowRunRepository : IWorkflowRunRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public WorkflowRunRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public void SaveWorkflowRun(WorkflowRun workflowRun)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(workflowRun);
                    transaction.Commit();
                }
            }

            Console.WriteLine($"Saving workflowRun:{workflowRun.RunId}");
        }
    }
}
