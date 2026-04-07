
namespace iWallet.Application.Interface
{
    public interface ITransactionRepository
    {
        Task MakeDepositAsync(int walletId , decimal ammount);
    }
}
