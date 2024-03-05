using System.Collections;
using System.Reflection;
using HRManagement.BuildingBlocks.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace HRManagement.BuildingBlocks.Caching;

public class MemoryCacheService : ICacheService
{
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;
    private readonly IMemoryCache _memoryCache;

    public MemoryCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromSeconds(60))
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(3600))
            .SetPriority(CacheItemPriority.Normal)
            .SetSize(1024);
    }

    public T Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }

    public void Set<T>(string key, T value)
    {
        _memoryCache.Set(key, value, _cacheEntryOptions);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public void RemoveAll(Func<string, bool> condition)
    {
        foreach (var key in GetAllKeys().Where(condition))
            Remove(key);
    }

    private IEnumerable<string> GetAllKeys()
    {
        var coherentState =
            typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);
        var coherentStateValue = coherentState.GetValue(_memoryCache);
        var entriesCollection = coherentStateValue.GetType()
            .GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        var entriesCollectionValue = entriesCollection.GetValue(coherentStateValue) as ICollection;

        if (entriesCollectionValue == null) return default;

        var keys = new List<string>();
        foreach (var item in entriesCollectionValue)
        {
            var methodInfo = item.GetType().GetProperty("Key");
            var val = methodInfo.GetValue(item);
            keys.Add(val.ToString());
        }

        return keys;
    }
}