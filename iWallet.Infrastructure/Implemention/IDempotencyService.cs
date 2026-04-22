
using StackExchange.Redis;

namespace iWallet.Infrastructure.Implemention
{
    public class IDempotencyService : IIDempotencyService
    {
        private readonly IDatabase _redis;
        public IDempotencyService(IConnectionMultiplexer connection)
        {
            _redis = connection.GetDatabase();
        }
        public async Task<bool> CreateRequestAsync(string key, TimeSpan expier)
        {
            // Atmoic Opration + reject any multible request work in the same time

            return await _redis.StringSetAsync
                (key,
                "PROCESSING",
                expier,
                 When.NotExists
                );
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _redis.StringGetAsync(key);
        }

        public async Task RemoveRequestAsync(string key)
        {
            await _redis.KeyDeleteAsync(key);
        }

        public async Task SetResopnseAsync(string key, string response, TimeSpan expier)
        {
             await _redis.StringSetAsync(key, response, expier);
        }
    }
}
