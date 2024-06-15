﻿using Sample.Services;
using SimpleCommander.Commands;
using SimpleCommander.Services;
using System;
using System.Threading.Tasks;

namespace Sample.Commands.ShareServiceConnection;

public class ShareServiceConnectionCommandHandler : ICommandHandler<ShareServiceConnectionCommandArgs>
{
    private readonly CLILogger _log;
    private readonly AdoApi _adoApi;

    public ShareServiceConnectionCommandHandler(CLILogger log, AdoApi adoApi)
    {
        _log = log;
        _adoApi = adoApi;
    }

    public async Task Handle(ShareServiceConnectionCommandArgs args)
    {
        if (args is null)
        {
            throw new ArgumentNullException(nameof(args));
        }

        _log.LogInformation("Sharing Service Connection...");

        var adoTeamProjectId = await _adoApi.GetTeamProjectId(args.AdoOrg, args.AdoTeamProject);

        if (await _adoApi.ContainsServiceConnection(args.AdoOrg, args.AdoTeamProject, args.ServiceConnectionId))
        {
            _log.LogInformation("Service connection already shared with team project");
            return;
        }

        await _adoApi.ShareServiceConnection(args.AdoOrg, args.AdoTeamProject, adoTeamProjectId, args.ServiceConnectionId);

        _log.LogSuccess("Successfully shared service connection");
    }
}
