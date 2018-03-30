using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using NLog;

namespace CodeGeneration.Services.Cache
{
    public class MemoryCacheService : ICacheService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private ObjectCache _cache;

        public MemoryCacheService(Type callerType)
        {
            Logger.Debug("Creating new instance of {0} from caller '{1}'", GetType().Name, callerType.FullName);
            _cache = new MemoryCache(callerType?.FullName ?? new Guid().ToString());
        }

        public ObjectCache Get()
        {
            return _cache;
        }

        public IDictionary<string, T> Get<T>()
        {
            return _cache.ToDictionary(kvp => kvp.Key, kvp => (T) kvp.Value, StringComparer.OrdinalIgnoreCase);
        }

        public T Get<T>(string key)
        {
            try
            {
                if (!Exists(key))
                {
                    return default(T);
                }

                return (T) _cache[key];
            }
            catch
            {
                return default(T);
            }
        }

        public string BuildCacheKey(string modelName, string templateName = "")
        {
            return string.IsNullOrWhiteSpace(templateName)
                ? modelName
                : $"{modelName}:{templateName}";
        }

        public void Set<T>(string key, T value)
        {
            _cache.Set(key, value, new CacheItemPolicy { AbsoluteExpiration = ObjectCache.InfiniteAbsoluteExpiration });
        }

        public void SetWithDuration<T>(string key, T value, int durationSeconds = 3600)
        {
            _cache.Set(key, value, new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 0, durationSeconds) });
        }

        public void SetWithExpiration<T>(string key, T value, DateTime expiration)
        {
            _cache.Set(key, value, new CacheItemPolicy { AbsoluteExpiration = expiration });
        }

        public bool Exists(string key)
        {
            return _cache.Contains(key);
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Clear()
        {
            _cache = MemoryCache.Default;
        }

    }
}
