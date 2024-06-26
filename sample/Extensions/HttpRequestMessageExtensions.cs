﻿using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Sample.Extensions;

public static class HttpRequestMessageExtensions
{
    public static HttpRequestMessage AddHeaders(this HttpRequestMessage request, Dictionary<string, string> headers)
    {
        headers?.ToList().ForEach(kv => request.Headers.Add(kv.Key, kv.Value));
        return request;
    }
}
