using SimpleCommander.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Sample.Services;

public class BasicHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly CLILogger _log;

    public BasicHttpClient(CLILogger log, HttpClient httpClient)
    {
        _log = log;
        _httpClient = httpClient;

        if (_httpClient != null)
        {
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OctoshiftCLI", GetCurrentVersion()));
        }
    }

    public async Task<string> GetAsync(string url)
    {
        _log.LogVerbose($"HTTP GET: {url}");

        var response = await _httpClient.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        _log.LogVerbose($"RESPONSE ({response.StatusCode}): {content}");

        response.EnsureSuccessStatusCode();

        return content;
    }

    private string GetCurrentVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    }
}
