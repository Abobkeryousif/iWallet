namespace iWallet.Application.DTOs
{
    public class WalletDto
    {
    }

    public record CreateWalletDto
    {
        public int UserId { get; set; }
        public WalletType WalletType { get; set; }
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Pin must be 4 digit")]
        public string pin { get; set; }
    }


    public record UpdateWalletBalanceDto
    {
        public decimal Balance { get; set; }
    }

    public record GetWalletDto
    {
        public string WalletNumber { get; set; }
        public decimal Balance { get; set; }
        public string WalletType { get; set; }
        public string Status { get; set; }

        // i make enum string to get accutal value 
    }
}
