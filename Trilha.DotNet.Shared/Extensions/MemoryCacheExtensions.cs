namespace Trilha.DotNet.Shared.Extensions;

public static class MemoryCacheExtensions
{
    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public static async Task<T> TryAddOrGetValue<T>(
        this IMemoryCache cache
        , string key
        , T data
        , TimeSpan? expires = null
        , bool delete = true)
    {
        try
        {
            await Semaphore.WaitAsync();

            if (cache.TryGetValue(key, out var value))
            {
                if (delete) cache.Remove(key);
                return (T)value!;
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetPriority(CacheItemPriority.High)
                .SetAbsoluteExpiration(expires ?? TimeSpan.FromHours(8));

            cache.Set(key, data, cacheEntryOptions);

            return data;
        }
        finally
        {
            Semaphore.Release();
        }
    }

    public static string CreateCacheKey(this string[] args)
    {
        var key = string.Join("|", args);
        var toEncodeAsBytes = Encoding.UTF8.GetBytes(key);

        return Convert.ToBase64String(toEncodeAsBytes);
    }

    public static string[] GetArgsFromCacheKey(this string key)
    {
        var result = Encoding.UTF8.GetString(Convert.FromBase64String(key));
        return result.Split('|');
    }
}