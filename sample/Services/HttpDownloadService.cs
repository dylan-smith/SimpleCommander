using SimpleCommander.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("OctoshiftCLI.Tests")]
namespace Sample.Services;

public class HttpDownloadService
{
    private readonly CLILogger _log;
    private readonly HttpClient _httpClient;
    private readonly FileSystemProvider _fileSystemProvider;

    public HttpDownloadService(CLILogger log, HttpClient httpClient, FileSystemProvider fileSystemProvider)
    {
        _log = log;
        _httpClient = httpClient;
        _fileSystemProvider = fileSystemProvider;

        if (_httpClient is not null)
        {
            _httpClient.Timeout = TimeSpan.FromHours(1);
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OctoshiftCLI", GetCurrentVersion()));
        }
    }

    public virtual async Task DownloadToFile(string url, string file)
    {
        _log.LogVerbose($"HTTP GET: {url}");

        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
        _log.LogVerbose($"RESPONSE ({response.StatusCode}): <truncated>");

        response.EnsureSuccessStatusCode();

        await using var streamToReadFrom = await response.Content.ReadAsStreamAsync();
        await using var streamToWriteTo = _fileSystemProvider.Open(file, FileMode.Create);
        await _fileSystemProvider.CopySourceToTargetStreamAsync(streamToReadFrom, streamToWriteTo);
    }

    public virtual async Task<byte[]> DownloadToBytes(string url)
    {
        _log.LogVerbose($"HTTP GET: {url}");

        using var response = await _httpClient.GetAsync(url);
        _log.LogVerbose($"RESPONSE ({response.StatusCode}): <truncated>");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }

    private string GetCurrentVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    }
}
