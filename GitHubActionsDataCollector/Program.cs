using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using GitHubActionsDataCollector;
using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.Driver;
using NHibernate;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    // for now hardcode the owner and repo names
    var repoOwner = "jameschristou";
    var repoName = "GitHubActionsDataCollector";
    var workflowId = 111639860; // you can get this id using https://api.github.com/repos/jameschristou/GitHubActionsDataCollector/actions/workflows

    await services.GetRequiredService<WorkflowProcessor>().Process(repoOwner, repoName, workflowId);
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

Console.WriteLine("Finished");

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureServices((_, services) =>
        {
            services.AddHttpClient();
            services.AddTransient<IGitHubActionsApiClient, GitHubActionsApiClient>();
            services.AddTransient<IWorkflowRunProcessor, WorkflowRunProcessor>();
            services.AddTransient<IWorkflowRunJobsProcessor, WorkflowRunJobsProcessor>();
            services.AddTransient<IWorkflowRunRepository, WorkflowRunRepository>();
            services.AddTransient<IWorkflowRunJobsRepository, WorkflowRunJobsRepository>();
            services.AddSingleton<WorkflowProcessor>();

            // NHibernate session factory registration
            services.AddSingleton<ISessionFactory>(CreateNHibernateSessionFactory());
        });
}

ISessionFactory CreateNHibernateSessionFactory()
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