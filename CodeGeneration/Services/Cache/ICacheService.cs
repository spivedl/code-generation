using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace CodeGeneration.Services.Cache
{
    public interface ICacheService
    {
        ObjectCache Get();
        IDictionary<string, T> Get<T>();
        T Get<T>(string key);
        string BuildCacheKey(string modelName, string templateName = "");
        void Set<T>(string key, T value);
        void SetWithDuration<T>(string key, T value, int durationSeconds = 3600);
        void SetWithExpiration<T>(string key, T value, DateTime expiration);
        bool Exists(string key);
        void Remove(string key);
        void Clear();
    }
}