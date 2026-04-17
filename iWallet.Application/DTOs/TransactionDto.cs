
namespace iWallet.Application.DTOs
{
    public record TransactionDto
    {
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string TransactionStatus { get; set; }
    }

    public record TransferTransactionDto
    {
        public string toAccountNumber { get; set; }
        public decimal amount { get; set; }
    }

    public record WithdrawalDto(int walletId,decimal amount);

    
}
