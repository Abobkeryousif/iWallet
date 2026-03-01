
namespace iWallet.Application.Interface
{
    public interface IUnitofwork
    {
        public IUserRepository UserRepository { get; }
        public IOtpRepository OtpRepository { get; }
    }
}
