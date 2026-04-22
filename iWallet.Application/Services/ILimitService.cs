namespace iWallet.Application.Services
{
    public interface ILimitService
    {
        Task<UserLimit> GetUserLimitAsync(int userId);
    }
}
