using GitHubActionsDataCollector.Entities;
using NHibernate;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IRegisteredWorkflowRepository
    {
        public Task<RegisteredWorkflow> GetLeastRecentlyCheckedWorkflow();
        public Task Update(RegisteredWorkflow registeredWorkflow);
    }

    public class RegisteredWorkflowRepository : IRegisteredWorkflowRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public RegisteredWorkflowRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task<RegisteredWorkflow> GetLeastRecentlyCheckedWorkflow()
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var registeredWorkflow = await session.QueryOver<RegisteredWorkflow>()
                                                .OrderBy(x => x.LastCheckedAtUtc).Asc
                                                .Take(1)
                                                .SingleOrDefaultAsync();

                    if(registeredWorkflow == null)
                    {
                        Console.WriteLine("No registered workflows to process");
                    }
                    else
                    {
                        Console.WriteLine($"Retrieved workflow:{registeredWorkflow.Id}");
                    }

                    transaction.Commit();

                    return registeredWorkflow;
                }
            }
        }

        public async Task Update(RegisteredWorkflow registeredWorkflow)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    await session.UpdateAsync(registeredWorkflow);

                    transaction.Commit();
                }
            }
        }
    }
}
