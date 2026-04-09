namespace iWallet.Application.DTOs
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName => $"{FirstName} {LastName}";

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? City { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
    }

    public record UpdateUserEmailDto(string email);

    public record GetUsersDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName => $"{FirstName} {LastName}";

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? City { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }

    public record LoginDto
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
