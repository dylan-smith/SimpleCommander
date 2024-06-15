﻿using Newtonsoft.Json;

namespace Sample.Extensions;

public static class ObjectExtensions
{
    public static string ToJson(this object obj) =>
        obj is null ? null : JsonConvert.SerializeObject(obj);
}
