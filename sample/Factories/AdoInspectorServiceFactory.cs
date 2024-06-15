using Sample.Services;
using SimpleCommander.Services;

namespace Sample.Factories;

public class AdoInspectorServiceFactory
{
    private readonly CLILogger _cliLogger;
    private AdoInspectorService _instance;

    public AdoInspectorServiceFactory(CLILogger octoLogger) => _cliLogger = octoLogger;

    public virtual AdoInspectorService Create(AdoApi adoApi)
    {
        _instance ??= new(_cliLogger, adoApi);

        return _instance;
    }
}
