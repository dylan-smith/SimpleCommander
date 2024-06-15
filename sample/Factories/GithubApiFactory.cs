using Sample.Services;
using SimpleCommander.Services;
using System.Net.Http;

namespace Sample.Factories;

public class GithubApiFactory
{
    private const string DEFAULT_API_URL = "https://api.github.com";

    private readonly CLILogger _logger;
    private readonly HttpClient _client;
    private readonly EnvironmentVariableProvider _environmentVariableProvider;
    private readonly DateTimeProvider _dateTimeProvider;
    private readonly RetryPolicy _retryPolicy;

    public GithubApiFactory(CLILogger octoLogger, HttpClient client, EnvironmentVariableProvider environmentVariableProvider, DateTimeProvider dateTimeProvider, RetryPolicy retryPolicy)
    {
        _logger = octoLogger;
        _client = client;
        _environmentVariableProvider = environmentVariableProvider;
        _dateTimeProvider = dateTimeProvider;
        _retryPolicy = retryPolicy;
    }

    public virtual GithubApi Create(string apiUrl = null, string targetPersonalAccessToken = null)
    {
        apiUrl ??= DEFAULT_API_URL;
        targetPersonalAccessToken ??= _environmentVariableProvider.TargetGithubPersonalAccessToken();
        var githubClient = new GithubClient(_logger, _client, _retryPolicy, _dateTimeProvider, targetPersonalAccessToken);
        return new GithubApi(githubClient, apiUrl, _retryPolicy);
    }
}
