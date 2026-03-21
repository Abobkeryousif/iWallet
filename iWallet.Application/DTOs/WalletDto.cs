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
}
