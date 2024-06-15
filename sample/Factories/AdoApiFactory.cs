using Sample.Services;
using SimpleCommander.Services;
using System.Net.Http;

namespace Sample.Factories;

public class AdoApiFactory
{
    private const string DEFAULT_API_URL = "https://dev.azure.com";

    private readonly CLILogger _cliLogger;
    private readonly HttpClient _client;
    private readonly EnvironmentVariableProvider _environmentVariableProvider;
    private readonly RetryPolicy _retryPolicy;

    public AdoApiFactory(CLILogger octoLogger, HttpClient client, EnvironmentVariableProvider environmentVariableProvider, RetryPolicy retryPolicy)
    {
        _cliLogger = octoLogger;
        _client = client;
        _environmentVariableProvider = environmentVariableProvider;
        _retryPolicy = retryPolicy;
    }

    public virtual AdoApi Create(string adoServerUrl, string personalAccessToken)
    {
        adoServerUrl ??= DEFAULT_API_URL;
        personalAccessToken ??= _environmentVariableProvider.AdoPersonalAccessToken();
        var adoClient = new AdoClient(_cliLogger, _client, _retryPolicy, personalAccessToken);
        return new AdoApi(adoClient, adoServerUrl, _cliLogger);
    }

    public virtual AdoApi Create(string personalAccessToken)
    {
        return Create(null, personalAccessToken);
    }
}
