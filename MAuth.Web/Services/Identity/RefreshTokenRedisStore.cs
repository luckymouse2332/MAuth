using Microsoft.Extensions.Caching.Distributed;

namespace MAuth.Web.Services.Identity
{
    public class RefreshTokenRedisStore(IDistributedCache distributedCache) : ITokenStore
    {
        private readonly IDistributedCache _distributedCache = distributedCache;

        public async Task SaveTokenAsync(string key, string token, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(key))
                throw new ArgumentException("Token and key cannot be null or empty.");
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _distributedCache.SetStringAsync(key, token, options);
        }

        public async Task<string?> GetTokenAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key cannot be null or empty.");
            return await _distributedCache.GetStringAsync(key);
        }
    }
}
