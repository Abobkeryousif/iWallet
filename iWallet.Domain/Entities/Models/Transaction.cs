
namespace iWallet.Domain.Entities.Models
{
    public class Transaction : BaseEntity
    {
        public string Reference { get; set; }
        public int FromWalletId { get; set; }
        public Wallet FromWallet { get; set; }
        public int ToWalletId { get; set; }
        public Wallet ToWallet { get; set; }

        public decimal Amount { get; set; }

        [Column(TypeName = "nvarchar(40)")]
        public TransactionType TransactionType { get; set; }

        [Column(TypeName = "nvarchar(40)")]
        public TransactionStatus Status { get; set; }
        public string? Description { get; set; }
    }
}
