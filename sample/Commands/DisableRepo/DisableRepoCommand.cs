using Microsoft.Extensions.DependencyInjection;
using Sample.Factories;
using SimpleCommander.Commands;
using SimpleCommander.Services;
using System;
using System.CommandLine;

namespace Sample.Commands.DisableRepo;

public class DisableRepoCommand : CommandBase<DisableRepoCommandArgs, DisableRepoCommandHandler>
{
    public DisableRepoCommand() : base(
        name: "disable-ado-repo",
        description: "Disables the repo in Azure DevOps. This makes the repo non-readable for all." +
                     Environment.NewLine +
                     "Note: Expects ADO_PAT env variable or --ado-pat option to be set.")
    {
        AddOption(AdoOrg);
        AddOption(AdoTeamProject);
        AddOption(AdoRepo);
        AddOption(AdoPat);
        AddOption(Verbose);
    }

    public Option<string> AdoOrg { get; } = new("--ado-org")
    {
        IsRequired = true
    };
    public Option<string> AdoTeamProject { get; } = new("--ado-team-project")
    {
        IsRequired = true
    };
    public Option<string> AdoRepo { get; } = new("--ado-repo")
    {
        IsRequired = true
    };
    public Option<string> AdoPat { get; } = new("--ado-pat");
    public Option<bool> Verbose { get; } = new("--verbose");

    public override DisableRepoCommandHandler BuildHandler(DisableRepoCommandArgs args, IServiceProvider sp)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        if (sp is null)
        {
            throw new ArgumentNullException(nameof(sp));
        }

        var log = sp.GetRequiredService<CLILogger>();
        var adoApiFactory = sp.GetRequiredService<AdoApiFactory>();
        var adoApi = adoApiFactory.Create(args.AdoPat);

        return new DisableRepoCommandHandler(log, adoApi);
    }
}
