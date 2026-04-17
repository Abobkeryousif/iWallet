
namespace iWallet.Application.Interface
{
    public interface IUnitofwork
    {
        public IUserRepository UserRepository { get; }
        public IOtpRepository OtpRepository { get; }
        public IWalletRepository WalletRepository { get; }
        public ITransactionRepository TransactionRepository { get; }
        public IBeneficiaryRepository BeneficiaryRepository { get; }
    }
}
