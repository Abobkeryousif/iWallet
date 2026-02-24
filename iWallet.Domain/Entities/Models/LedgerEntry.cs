namespace iWallet.Domain.Entities.Models
{
    public class LedgerEntry : BaseEntity
    {
        public int WalletId { get; set; }
        public Wallet Wallet { get; set; }
        public int TransactionId { get; set; }
        public Transaction Transaction { get; set; }

        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public string Particulars { get; set; }
    }
}
