using Sample.Services;
using SimpleCommander.Commands;
using SimpleCommander.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sample.Commands.DisableRepo;

public class DisableRepoCommandHandler : ICommandHandler<DisableRepoCommandArgs>
{
    private readonly CLILogger _log;
    private readonly AdoApi _adoApi;

    public DisableRepoCommandHandler(CLILogger log, AdoApi adoApi)
    {
        _log = log;
        _adoApi = adoApi;
    }

    public async Task Handle(DisableRepoCommandArgs args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        _log.LogInformation("Disabling repo...");

        var allRepos = await _adoApi.GetRepos(args.AdoOrg, args.AdoTeamProject);
        if (allRepos.Any(r => r.Name == args.AdoRepo && r.IsDisabled))
        {
            _log.LogSuccess($"Repo '{args.AdoOrg}/{args.AdoTeamProject}/{args.AdoRepo}' is already disabled - No action will be performed");
            return;
        }
        var repoId = allRepos.First(r => r.Name == args.AdoRepo).Id;
        await _adoApi.DisableRepo(args.AdoOrg, args.AdoTeamProject, repoId);

        _log.LogSuccess("Repo successfully disabled");
    }
}
