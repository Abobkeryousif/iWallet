
namespace iWallet.Application.DTOs
{
    public record TransactionDto { }

    public record TransferTransactionDto
    {
        public string toAccountNumber { get; set; }
        public decimal amount { get; set; }
    }

    public record WithdrawalDto(int walletId,decimal amount);
}
