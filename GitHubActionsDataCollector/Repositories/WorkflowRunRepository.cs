using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using GitHubActionsDataCollector.Entities;
using NHibernate;
using NHibernate.Driver;

namespace GitHubActionsDataCollector.Repositories
{
    public interface IWorkflowRunRepository
    {
        public void SaveWorkflowRun(WorkflowRun workflowRun);
    }

    public class WorkflowRunRepository : IWorkflowRunRepository
    {
        public void SaveWorkflowRun(WorkflowRun workflowRun)
        {
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.Save(workflowRun);
                    transaction.Commit();
                }
            }

            Console.WriteLine($"Saving workflowRun:{workflowRun.RunId}");
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(
                MsSqlConfiguration.MsSql2012
                    .Driver<MicrosoftDataSqlClientDriver>()
                    .ConnectionString("Server=.\\SQLEXPRESS;Database=GHAData;Integrated Security=True;Encrypt=false"))
              .Mappings(m =>
                m.FluentMappings.AddFromAssemblyOf<Program>())
              .BuildSessionFactory();
        }
    }
}
