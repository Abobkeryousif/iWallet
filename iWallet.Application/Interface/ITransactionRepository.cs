
namespace iWallet.Application.Interface
{
    public interface ITransactionRepository
    {
        Task<string> MakeDepositAsync(int walletId , decimal ammount);
        Task<string> TransferAsync(string toAccountNumber, decimal amount, int userId);
        Task<string> MakeWithdrawal(int walletId, decimal amount);
        Task<List<TransactionDto>> TransactionHistory(int walletId);

    }
}
