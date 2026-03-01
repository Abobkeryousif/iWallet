
namespace iWallet.Application.Interface
{
    public interface IUserRepository
    {
        Task<string> UserRegister(UserDto userDto);
    }
}
