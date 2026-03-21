namespace iWallet.Application.Interface
{
    public interface IWalletRepository
    {
        Task<string> CreateAsync(CreateWalletDto walletDto);
    }
}
