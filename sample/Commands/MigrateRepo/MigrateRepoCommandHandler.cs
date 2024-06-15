using Sample.Extensions;
using Sample.Models;
using Sample.Services;
using SimpleCommander;
using SimpleCommander.Commands;
using SimpleCommander.Extensions;
using SimpleCommander.Services;
using System;
using System.Threading.Tasks;

namespace Sample.Commands.MigrateRepo;

public class MigrateRepoCommandHandler : ICommandHandler<MigrateRepoCommandArgs>
{
    private readonly CLILogger _log;
    private readonly GithubApi _githubApi;
    private readonly EnvironmentVariableProvider _environmentVariableProvider;

    public MigrateRepoCommandHandler(CLILogger log, GithubApi githubApi, EnvironmentVariableProvider environmentVariableProvider)
    {
        _log = log;
        _githubApi = githubApi;
        _environmentVariableProvider = environmentVariableProvider;
    }

    public async Task Handle(MigrateRepoCommandArgs args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        _log.LogInformation("Migrating Repo...");

        args.GithubPat ??= _environmentVariableProvider.TargetGithubPersonalAccessToken();

        var adoRepoUrl = GetAdoRepoUrl(args.AdoOrg, args.AdoTeamProject, args.AdoRepo, args.AdoServerUrl);

        args.AdoPat ??= _environmentVariableProvider.AdoPersonalAccessToken();
        var githubOrgId = await _githubApi.GetOrganizationId(args.GithubOrg);

        string migrationSourceId;

        try
        {
            migrationSourceId = await _githubApi.CreateAdoMigrationSource(githubOrgId, args.AdoServerUrl);
        }
        catch (CLIException ex) when (ex.Message.Contains("not have the correct permissions to execute"))
        {
            var message = $"{ex.Message} Insufficient Permissions";
            throw new CLIException(message, ex);
        }

        string migrationId;

        try
        {
            migrationId = await _githubApi.StartMigration(migrationSourceId, adoRepoUrl, githubOrgId, args.GithubRepo, args.AdoPat, args.GithubPat, targetRepoVisibility: args.TargetRepoVisibility);
        }
        catch (CLIException ex)
        {
            if (ex.Message == $"A repository called {args.GithubOrg}/{args.GithubRepo} already exists")
            {
                _log.LogWarning($"The Org '{args.GithubOrg}' already contains a repository with the name '{args.GithubRepo}'. No operation will be performed");
                return;
            }

            throw;
        }

        if (args.QueueOnly)
        {
            _log.LogInformation($"A repository migration (ID: {migrationId}) was successfully queued.");
            return;
        }

        var (migrationState, _, warningsCount, failureReason, migrationLogUrl) = await _githubApi.GetMigration(migrationId);

        while (RepositoryMigrationStatus.IsPending(migrationState))
        {
            _log.LogInformation($"Migration in progress (ID: {migrationId}). State: {migrationState}. Waiting 10 seconds...");
            await Task.Delay(10000);
            (migrationState, _, warningsCount, failureReason, migrationLogUrl) = await _githubApi.GetMigration(migrationId);
        }

        var migrationLogAvailableMessage = $"Migration log available at {migrationLogUrl} or by running `gh download-logs --github-org {args.GithubOrg} --github-repo {args.GithubRepo}`";

        if (RepositoryMigrationStatus.IsFailed(migrationState))
        {
            _log.LogError($"Migration Failed. Migration ID: {migrationId}");
            _log.LogInformation(migrationLogAvailableMessage);
            throw new CLIException(failureReason);
        }

        _log.LogSuccess($"Migration completed (ID: {migrationId})! State: {migrationState}");
        _log.LogInformation(migrationLogAvailableMessage);
    }

    private string GetAdoRepoUrl(string org, string project, string repo, string serverUrl)
    {
        serverUrl = serverUrl.HasValue() ? serverUrl.TrimEnd('/') : "https://dev.azure.com";
        return $"{serverUrl}/{org.EscapeDataString()}/{project.EscapeDataString()}/_git/{repo.EscapeDataString()}";
    }
}
