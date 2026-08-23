using iWallet.Application.Common;

namespace iWallet.Application.Interface
{
    public interface ITransactionRepository
    {
        Task<Result<string>> MakeDepositAsync(DepositDto depositDto);
        Task<Result<string>> TransferAsync(string toAccountNumber, decimal amount, int userId);
        Task<Result<string>> MakeWithdrawal(int walletId, decimal amount);
        Task<List<TransactionDto>> TransactionHistory(int walletId);
        Task<string> TransferToBeneficiery(string beneficieryName, decimal amount, int userId);
        Task<decimal> GetTransactionsTodayTotalAsync(int userId);

    }
}
