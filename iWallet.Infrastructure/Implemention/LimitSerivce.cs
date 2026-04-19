using Microsoft.Extensions.Caching.Memory;

namespace iWallet.Infrastructure.Implemention
{
    public class LimitSerivce : ILimitService
    {
        private readonly IMemoryCache _cache;
        private readonly ApplicationDbContext _context;

        public LimitSerivce(IMemoryCache cache, ApplicationDbContext context)
        {
            _cache = cache;
            _context = context;
        }
        public async Task<UserLimit> GetUserLimitAsync(int userId)
        {
            var cacheKey = $"user_limits_{userId}";

            if (_cache.TryGetValue(cacheKey, out UserLimit cachedLimits))
                return cachedLimits;

            var limits = await _context.UserLimits.FirstOrDefaultAsync(u=> u.UserId == userId);

            if (limits == null)
            {
                limits = new UserLimit
                {
                    UserId = userId,
                    PerTransactionLimit = 5000,
                    DailyLimit = 20000
                };

                _context.UserLimits.Add(limits);
                await _context.SaveChangesAsync();

            }

            _cache.Set(cacheKey, limits, TimeSpan.FromMinutes(10));

            return limits;
        }
    }
}
