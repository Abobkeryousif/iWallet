
namespace iWallet.Application.Interface
{
    public interface ITransactionRepository
    {
        Task<Result<string>> MakeDepositAsync(DepositDto depositDto, int userId);
        Task<Result<string>> TransferAsync(string toAccountNumber, decimal amount, int userId);
        Task<Result<string>> MakeWithdrawal(int walletId, int userId, decimal amount);
        Task<List<TransactionDto>> TransactionHistory(int walletId, int userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<Result<string>> TransferToBeneficiery(string beneficieryName, decimal amount, int userId);
        Task<decimal> GetTransactionsTodayTotalAsync(int userId);

    }
}
