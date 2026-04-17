namespace iWallet.Domain.Entities.Models
{
    public class Beneficiary
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string WalletNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
