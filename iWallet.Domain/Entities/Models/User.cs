namespace iWallet.Domain.Entities.Models
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName => $"{FirstName} {LastName}";

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? City { get; set; }

        [DataType(DataType.Date)]
        [Range(1960, 2008)]
        public DateTime BirthDate { get; set; }
        public string? ProfileImageUrl { get; set; }

        public bool IsActive { get; set; } = false;
        public string Password { get; set; }
        [NotMapped]
        [Compare("Password")]
        public string confirmPassword { get; set; }

        public string? Role { get; set; }

        public ICollection<Wallet> Wallets { get; set; }  
    }
}
