
namespace iWallet.Infrastructure.Implemention
{
    public class Unitofwork : IUnitofwork
    {
        public Unitofwork(ILogger<UserRepository> userLogger, ILogger<TransactionRepository> transactionLogger ,ApplicationDbContext context, ISendEmailService sendEmail, IOtpRepository otpRepository, ITokenService tokenService, ILimitService limitService)
        {
            UserRepository = new UserRepository(userLogger ,context, sendEmail, otpRepository, tokenService);
            OtpRepository = new OtpRepository(context, sendEmail);
            WalletRepository = new WalletRepository(context);
            TransactionRepository = new TransactionRepository(transactionLogger ,context, limitService);
            BeneficiaryRepository = new BeneficiaryRepository(context);
        }

        public IUserRepository UserRepository { get; }

        public IOtpRepository OtpRepository { get; }

        public IWalletRepository WalletRepository { get; }

        public ITransactionRepository TransactionRepository { get; }

        public IBeneficiaryRepository BeneficiaryRepository { get; }
    }
}
