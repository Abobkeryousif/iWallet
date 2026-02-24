
namespace iWallet.Domain.Entities.Models
{
    public class Transaction : BaseEntity
    {
        public string Reference { get; set; }
        public int? FromWalletId { get; set; }
        public Wallet? FromWallet { get; set; }
        public int? ToWalletId { get; set; }
        public Wallet? ToWallet { get; set; }

        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }

        public TransactionStatus Status { get; set; }
        public string? Description { get; set; }
    }
}
