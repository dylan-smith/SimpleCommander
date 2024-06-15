using Microsoft.Extensions.DependencyInjection;
using Sample.Factories;
using Sample.Services;
using SimpleCommander.Extensions;
using SimpleCommander.Services;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OctoshiftCLI.Tests")]
namespace Sample;

public static class Program
{
    private static readonly CLILogger Logger = new();

    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        RegisterServices(serviceCollection);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var parser = BuildParser(serviceProvider);

        await parser.InvokeAsync(args);
    }

    private static void RegisterServices(ServiceCollection serviceCollection)
    {
        serviceCollection
            .AddSingleton(Logger)
            .AddSingleton<EnvironmentVariableProvider>()
            .AddSingleton<AdoApiFactory>()
            .AddSingleton<GithubApiFactory>()
            .AddSingleton<RetryPolicy>()
            .AddSingleton<BasicHttpClient>()
            .AddSingleton<GithubStatusApi>()
            .AddSingleton<HttpDownloadServiceFactory>()
            .AddSingleton<OrgsCsvGeneratorService>()
            .AddSingleton<TeamProjectsCsvGeneratorService>()
            .AddSingleton<ReposCsvGeneratorService>()
            .AddSingleton<PipelinesCsvGeneratorService>()
            .AddSingleton<AdoInspectorService>()
            .AddSingleton<AdoInspectorServiceFactory>()
            .AddSingleton<DateTimeProvider>()
            .AddSingleton<FileSystemProvider>()
            .AddHttpClient();
    }

    private static Parser BuildParser(ServiceProvider serviceProvider)
    {
        var root = new RootCommand("Automate end-to-end Azure DevOps Repos to GitHub migrations.")
            .AddCommands(serviceProvider);
        var commandLineBuilder = new CommandLineBuilder(root);

        return commandLineBuilder
            .UseDefaults()
            .UseExceptionHandler((ex, _) =>
            {
                Logger.LogError(ex);
                Environment.ExitCode = 1;
            }, 1)
            .Build();
    }
}
