namespace iWallet.Infrastructure.Implemention
{
    public class Unitofwork : IUnitofwork
    {
        public Unitofwork(ApplicationDbContext context)
        {
            UserRepository = new UserRepository(context);
            OtpRepository = new OtpRepository(context);
        }

        public IUserRepository UserRepository { get; }

        public IOtpRepository OtpRepository {  get; }
    }
}
