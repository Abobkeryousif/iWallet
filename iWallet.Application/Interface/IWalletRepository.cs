namespace iWallet.Application.Interface
{
    public interface IWalletRepository
    {
        Task<string> CreateAsync(CreateWalletDto walletDto);
        Task<List<GetWalletDto>> GetWalletsAsync();
        Task<GetWalletDto> GetWalletById(int walletId);
        Task<GetWalletDto> GetByWalletNumber(string walletNumber);
        Task<string> PatchWalletBalance(int walletId, decimal balance);
    }
}
