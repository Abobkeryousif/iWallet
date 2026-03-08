
namespace iWallet.Application.Interface
{
    public interface IUserRepository
    {
        Task<string> UserRegister(UserDto userDto);
        string CompleteRegister(string Otp);
        string ResetEmail (int id ,UpdateUserEmailDto updateUserEmail);
        Task<List<GetUsersDto>> GetAllUsers();
    }
}
