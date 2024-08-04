using GitHubActionsDataCollector;
using GitHubActionsDataCollector.GitHubActionsApiClient;
using GitHubActionsDataCollector.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    // for now hardcode the owner and repo names
    var repoOwner = "";
    var repoName = "";

    await services.GetRequiredService<Processor>().Run(repoOwner, repoName);
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
            services.AddSingleton<Processor>();
        });
}