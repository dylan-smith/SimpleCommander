using Sample.Services;
using SimpleCommander.Commands;
using SimpleCommander.Services;
using System;
using System.Threading.Tasks;

namespace Sample.Commands.AddTeamToRepo;

public class AddTeamToRepoCommandHandler : ICommandHandler<AddTeamToRepoCommandArgs>
{
    private readonly CLILogger _logger;
    private readonly GithubApi _githubApi;

    public AddTeamToRepoCommandHandler(CLILogger logger, GithubApi githubApi)
    {
        _logger = logger;
        _githubApi = githubApi;
    }

    public async Task Handle(AddTeamToRepoCommandArgs args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        _logger.LogInformation("Adding team to repo...");

        var teamSlug = await _githubApi.GetTeamSlug(args.GithubOrg, args.Team);
        await _githubApi.AddTeamToRepo(args.GithubOrg, args.GithubRepo, teamSlug, args.Role);

        _logger.LogSuccess("Successfully added team to repo");
    }
}
