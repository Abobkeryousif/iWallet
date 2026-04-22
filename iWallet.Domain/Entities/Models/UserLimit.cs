namespace iWallet.Domain.Entities.Models
{
    public class UserLimit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal PerTransactionLimit { get; set; }
        public decimal DailyLimit { get; set; }
    }
}
