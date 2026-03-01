namespace iWallet.Infrastructure.Implemention
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationDbContext _context;
        public OtpRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool CreateOtp(Otp otp)
        {
            _context.OTPs.Add(otp);
            return _context.SaveChanges() > 0;
        }

        public bool UpdateOtp(Otp otp)
        {
            _context.OTPs.Update(otp);
            return _context.SaveChanges() > 0 ;
        }
    }
}
