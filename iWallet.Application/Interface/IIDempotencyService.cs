namespace iWallet.Application.Interface
{
    public interface IIDempotencyService
    {
        Task<bool> CreateRequestAsync(string key, TimeSpan expier);
        Task<string?> GetAsync(string key);
        Task SetResopnseAsync(string key, string response, TimeSpan expier);
        Task RemoveRequestAsync(string key);
    }
}
