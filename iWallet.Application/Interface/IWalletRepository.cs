namespace iWallet.Application.Interface
{
    public interface IWalletRepository
    {
        Task<string> CreateAsync(int userId, WalletType walletType, string pin);

        Task<List<GetWalletDto>> GetUserWalletsAsync(int userId);
        Task<List<GetWalletDto>> GetWalletsAsync();
        Task<GetWalletDto> GetWalletById(int walletId);
        Task<GetWalletDto> GetByWalletNumber(string walletNumber);
        Task<string> PatchWalletBalance(int walletId, decimal balance);
    }
}
