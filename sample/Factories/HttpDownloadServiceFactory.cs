using Sample.Services;
using SimpleCommander.Services;
using System.Net.Http;

namespace Sample.Factories;

public sealed class HttpDownloadServiceFactory
{
    private readonly CLILogger _log;
    private readonly IHttpClientFactory _clientFactory;
    private readonly FileSystemProvider _fileSystemProvider;

    public HttpDownloadServiceFactory(CLILogger log, IHttpClientFactory clientFactory, FileSystemProvider fileSystemProvider)
    {
        _log = log;
        _clientFactory = clientFactory;
        _fileSystemProvider = fileSystemProvider;
    }

    public HttpDownloadService CreateDefaultWithRedirects()
    {
        var httpClient = _clientFactory.CreateClient();

        return new HttpDownloadService(_log, httpClient, _fileSystemProvider);
    }

    public HttpDownloadService CreateDefault()
    {
        var httpClient = _clientFactory.CreateClient("Default");

        return new HttpDownloadService(_log, httpClient, _fileSystemProvider);
    }

    public HttpDownloadService CreateClientNoSsl()
    {
        var httpClient = _clientFactory.CreateClient("NoSSL");

        return new HttpDownloadService(_log, httpClient, _fileSystemProvider);
    }
}

