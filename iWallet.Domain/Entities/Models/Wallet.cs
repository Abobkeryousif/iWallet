namespace iWallet.Domain.Entities.Models
{
    public class Wallet : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public string WalletNumber { get; set; } 

        [Column(TypeName = "nvarchar(40)")]
        public WalletType WalletType { get; set; }
        public decimal Balance { get; set; }

        [Column(TypeName = "nvarchar(40)")]
        public WalletStatus Status { get; set; }
        public byte[] PinHash { get; set; }
        public byte[] PinSalt { get; set; }

        public ICollection<Transaction> Transactions { get; set; }

    }

}
