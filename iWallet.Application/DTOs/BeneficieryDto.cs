
namespace iWallet.Application.DTOs
{
    public record BeneficieryDto
    {
        public string Name { get; set; }
        public string WalletNumber { get; set; }
    }

    public record UpdateBeneficieryName
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string Name { get; set; }
    }  
}
