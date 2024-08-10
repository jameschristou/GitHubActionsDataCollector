using GitHubActionsDataCollector.Entities;
using NHibernate;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IRegisteredWorkflowRepository
    {
        public Task<RegisteredWorkflow> GetNextWorkflow();
    }

    public class RegisteredWorkflowRepository : IRegisteredWorkflowRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public RegisteredWorkflowRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task<RegisteredWorkflow> GetNextWorkflow()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var workflow = await session.QueryOver<RegisteredWorkflow>()
                                                .OrderBy(x => x.LastCheckedAtUtc).Desc
                                                .SingleOrDefaultAsync();
                    transaction.Commit();

                    Console.WriteLine($"Retrieved workflow:{workflow.Id}");

                    return workflow;
                }
            }
        }
    }
}
