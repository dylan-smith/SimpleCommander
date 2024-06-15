using SimpleCommander.Commands;

namespace Sample.Commands.ConfigureAutoLink;

public class ConfigureAutoLinkCommandArgs : CommandArgs
{
    public string GithubOrg { get; set; }
    public string GithubRepo { get; set; }
    public string AdoOrg { get; set; }
    public string AdoTeamProject { get; set; }
    [Secret]
    public string GithubPat { get; set; }
}
