﻿using SimpleCommander.Commands;

namespace Sample.Commands.IntegrateBoards;

public class IntegrateBoardsCommandArgs : CommandArgs
{
    public string AdoOrg { get; set; }
    public string AdoTeamProject { get; set; }
    public string GithubOrg { get; set; }
    public string GithubRepo { get; set; }
    [Secret]
    public string AdoPat { get; set; }
    [Secret]
    public string GithubPat { get; set; }
}
