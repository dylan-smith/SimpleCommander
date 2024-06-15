﻿using Newtonsoft.Json.Linq;
using Sample.Extensions;
using SimpleCommander.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sample.Services;

public class BbsClient
{
    private const int DEFAULT_PAGE_SIZE = 100;
    private readonly HttpClient _httpClient;
    private readonly CLILogger _logger;
    private readonly RetryPolicy _retryPolicy;

    public BbsClient(CLILogger logger, HttpClient httpClient, RetryPolicy retryPolicy, string username, string password) :
        this(logger, httpClient, retryPolicy)
    {
        if (_httpClient != null)
        {
            var authCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authCredentials);
        }
    }

    public BbsClient(CLILogger logger, HttpClient httpClient, RetryPolicy retryPolicy)
    {
        _logger = logger;
        _httpClient = httpClient;
        _retryPolicy = retryPolicy;

        if (_httpClient != null)
        {
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("OctoshiftCLI", GetCurrentVersion()));
        }
    }

    public virtual async Task<string> GetAsync(string url)
    {
        return await _retryPolicy.Retry(async () => await SendAsync(HttpMethod.Get, url));
    }

    public virtual async IAsyncEnumerable<JToken> GetAllAsync(string url)
    {
        var hasNextPage = true;
        var nextPageStart = 0;
        while (hasNextPage)
        {
            var response = await GetWithPagination(url, nextPageStart);
            var jResponse = JObject.Parse(response);

            foreach (var jToken in jResponse["values"]!)
            {
                yield return jToken;
            }

            hasNextPage = !jResponse["isLastPage"]?.ToObject<bool>() ?? false;
            nextPageStart = jResponse["nextPageStart"]?.ToObject<int>() ?? 0;
        }
    }

    public virtual async Task<string> PostAsync(string url, object body) => await SendAsync(HttpMethod.Post, url, body);

    public virtual async Task<string> DeleteAsync(string url) => await SendAsync(HttpMethod.Delete, url);

    private async Task<string> GetWithPagination(string url, int start = 0, int limit = DEFAULT_PAGE_SIZE) => await GetAsync(AddPaginationParams(url, start, limit));

    private async Task<string> SendAsync(HttpMethod httpMethod, string url, object body = null)
    {
        _logger.LogVerbose($"HTTP {httpMethod}: {url}");

        if (body != null)
        {
            _logger.LogVerbose($"HTTP BODY: {body.ToJson()}");
        }

        using var payload = body?.ToJson().ToStringContent();
        var response = httpMethod.ToString() switch
        {
            "GET" => await _httpClient.GetAsync(url),
            "DELETE" => await _httpClient.DeleteAsync(url),
            "POST" => await _httpClient.PostAsync(url, payload),
            "PUT" => await _httpClient.PutAsync(url, payload),
            "PATCH" => await _httpClient.PatchAsync(url, payload),
            _ => throw new ArgumentOutOfRangeException($"{httpMethod} is not supported.")
        };
        var content = await response.Content.ReadAsStringAsync();
        _logger.LogVerbose($"RESPONSE ({response.StatusCode}): {content}");

        response.EnsureSuccessStatusCode();

        return content;
    }

    private string AddPaginationParams(string url, int start, int limit)
    {
        var uri = new Uri(url);
        var path = uri.GetLeftPart(UriPartial.Path);
        var queryParams = HttpUtility.ParseQueryString(uri.Query);

        queryParams["start"] = start.ToString();
        queryParams["limit"] = limit.ToString();

        return $"{path}?{queryParams}";
    }

    private string GetCurrentVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    }
}
