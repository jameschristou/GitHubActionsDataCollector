using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using GitHubActionsDataCollector;
using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.Driver;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    // get the workflow to process
    // TODO enable processing of multiple workflows in a single execution of this application
    var workflow = await services.GetRequiredService<IRegisteredWorkflowRepository>().GetLeastRecentlyCheckedWorkflow();

    if (workflow == null)
    {
        Console.WriteLine("No workflows to process!");
        return;
    }

    await services.GetRequiredService<WorkflowProcessor>().Process(workflow);
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
            services.AddTransient<IRegisteredWorkflowRepository, RegisteredWorkflowRepository>();
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