
namespace iWallet.Application.Interface
{
    public interface ITransactionRepository
    {
        Task<string> MakeDepositAsync(int walletId , decimal ammount);
    }
}
