namespace iWallet.Application.Interface
{
    public interface IWalletRepository
    {
        Task<string> CreateAsync(int userId, WalletType walletType, string pin);

        Task<List<GetWalletDto>> GetUserWalletsAsync(int userId);
        Task<List<GetWalletDto>> GetWalletsAsync();
        Task<GetWalletDto> GetWalletById(int walletId, int user);
        Task<GetWalletDto> GetByWalletNumber(string walletNumber, int userId);
        Task<string> PatchWalletBalance(int walletId, int userId, decimal balance);
    }
}
