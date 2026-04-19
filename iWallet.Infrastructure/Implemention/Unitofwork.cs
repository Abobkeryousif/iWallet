
namespace iWallet.Infrastructure.Implemention
{
    public class Unitofwork : IUnitofwork
    {
        public Unitofwork(ApplicationDbContext context , ISendEmailService sendEmail,IOtpRepository otpRepository,ITokenService tokenService,ILimitService limitService)
        {
            UserRepository = new UserRepository(context,sendEmail,otpRepository,tokenService);
            OtpRepository = new OtpRepository(context, sendEmail);
            WalletRepository = new WalletRepository(context);
            TransactionRepository = new TransactionRepository(context,WalletRepository,limitService);
            BeneficiaryRepository = new BeneficiaryRepository(context);
        }

        public IUserRepository UserRepository { get; }

        public IOtpRepository OtpRepository {  get; }

        public IWalletRepository WalletRepository {  get; }

        public ITransactionRepository TransactionRepository {  get; }

        public IBeneficiaryRepository BeneficiaryRepository {  get; }
    }
}
