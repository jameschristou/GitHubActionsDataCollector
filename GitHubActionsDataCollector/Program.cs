﻿using GitHubActionsDataCollector;
using GitHubActionsDataCollector.GitHubActionsApiClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<Processor>().Run();
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
            services.AddTransient<IGitHubActionsApiClient, GitHubActionsApiClient>();
            services.AddTransient<IWorkflowRunProcessor, WorkflowRunProcessor>();
            services.AddSingleton<Processor>();
        });
}