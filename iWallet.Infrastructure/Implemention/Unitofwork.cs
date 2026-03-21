
namespace iWallet.Infrastructure.Implemention
{
    public class Unitofwork : IUnitofwork
    {
        public Unitofwork(ApplicationDbContext context , ISendEmailService sendEmail,IOtpRepository otpRepository)
        {
            UserRepository = new UserRepository(context,sendEmail,otpRepository);
            OtpRepository = new OtpRepository(context, sendEmail);
            WalletRepository = new WalletRepository(context);
        }

        public IUserRepository UserRepository { get; }

        public IOtpRepository OtpRepository {  get; }

        public IWalletRepository WalletRepository {  get; }
    }
}
