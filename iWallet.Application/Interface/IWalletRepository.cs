namespace iWallet.Application.Interface
{
    public interface IWalletRepository
    {
        Task<string> CreateAsync(int userId, WalletType walletType, string pin);

        Task<List<GetWalletDto>> GetUserWalletsAsync(int userId, CancellationToken cancellationToken);
        Task<List<GetWalletDto>> GetWalletsAsync(CancellationToken cancellationToken);
        Task<GetWalletDto> GetWalletById(int walletId, int user, CancellationToken cancellationToken);
        Task<GetWalletDto> GetByWalletNumber(string walletNumber, int userId, CancellationToken cancellationToken);
        Task<string> PatchWalletBalance(int walletId, int userId, decimal balance);
    }
}
