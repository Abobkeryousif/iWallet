using iWallet.Domain.Entities.Common;

namespace iWallet.Application.Services
{
    public interface ILimitService
    {
        Task<UserLimit> GetUserLimitAsync(int userId);
    }
}
