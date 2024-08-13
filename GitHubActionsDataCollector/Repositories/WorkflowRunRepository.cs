using GitHubActionsDataCollector.Entities;
using NHibernate;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IWorkflowRunRepository
    {
        public Task SaveWorkflowRun(WorkflowRun workflowRun);
    }

    public class WorkflowRunRepository : IWorkflowRunRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public WorkflowRunRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task SaveWorkflowRun(WorkflowRun workflowRun)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    await session.SaveAsync(workflowRun);
                    transaction.Commit();
                }
            }

            Console.WriteLine($"Saving workflowRun:{workflowRun.RunId}");
        }
    }
}
