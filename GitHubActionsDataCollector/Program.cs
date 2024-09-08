using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Cfg;
using GitHubActionsDataCollector.GitHubActionsApi;
using GitHubActionsDataCollector.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NHibernate.Driver;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Cfg;
using GitHubActionsDataCollector.Processors;
using GitHubActionsDataCollector;
using GitHubActionsDataCollector.Processors.JobProcessors;
using GitHubActionsDataCollector.Services;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    await services.GetRequiredService<Processor>().Run();
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
            services.AddTransient<IWorkflowRunJobProcessor, WorkflowRunJobProcessor>();
            services.AddSingleton<IWorkflowRunLogsService, WorkflowRunLogsService>();  // TODO: for now register as singleton but need to improve this for memory management

            // job processing services
            services.AddTransient<DotNetXmlTestResultsProcessor>();
            services.AddTransient<CypressTestResultsProcessor>();
            services.AddTransient<JobProcessorFactory>();
            services.AddSingleton<IJobProcessorMatchingService, JobProcessorMatchingService>();
            services.AddSingleton<RegExMatchProvider>();
            services.AddSingleton<ExactMatchProvider>();

            services.AddTransient<IWorkflowRunRepository, WorkflowRunRepository>();
            services.AddTransient<IWorkflowRunJobsRepository, WorkflowRunJobsRepository>();
            services.AddTransient<IRegisteredWorkflowRepository, RegisteredWorkflowRepository>();
            services.AddTransient<IRegisteredWorkflowProcessor, RegisteredWorkflowProcessor>();
            services.AddSingleton<Processor>();

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
      //.ExposeConfiguration(BuildSchema) // only uncomment this line to generate the schema. However this will drop existing tables and recreate them
      .BuildSessionFactory();
}

void BuildSchema(Configuration config)
{
    // this NHibernate tool takes a configuration (with mapping info in)
    // and exports a database schema from it. This basically drops the existing
    // tables and recreates them so be careful when using this!
    new SchemaExport(config)
      .Create(false, true);
}