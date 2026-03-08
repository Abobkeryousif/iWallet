namespace iWallet.Infrastructure.Implemention
{
    public class OtpRepository : IOtpRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ISendEmailService _sendEmailService;
        public OtpRepository(ApplicationDbContext context, ISendEmailService sendEmailService)
        {
            _context = context;
            _sendEmailService = sendEmailService;
        }
        public bool CreateOtp(Otp otp)
        {
            _context.OTPs.Add(otp);
            return _context.SaveChanges() > 0;
        }

        public string ResendOtp(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
                return "email is requierd";

            var user = _context.Users.FirstOrDefault(e=> e.Email == userEmail);
            if (user == null)
                return $"not found user with this email: {userEmail}";

            var oldOtp = _context.OTPs.FirstOrDefault(o=> o.UserEmail == user.Email && !o.IsUsed && o.ExpirationOn > DateTime.Now);
            if (oldOtp != null)
            {
               oldOtp.IsUsed = true;
               _context.OTPs.Update(oldOtp);    
            }

            Random random = new Random();
            int otp = random.Next(0, 9999);

            var userOtp = new Otp
            {
                otp = otp.ToString("0000"),
                UserEmail = user.Email,
                ExpirationOn = DateTime.Now.AddMinutes(5),  
                IsUsed = false
            };

            CreateOtp(userOtp);

            _sendEmailService.SendEmail(user.Email, "Confierm Register", $"Please Confierm this code to complete register {userOtp.otp}");

            return "Resend Otp Complete , confierm your account";
        }

        public bool UpdateOtp(Otp otp)
        {
            _context.OTPs.Update(otp);
            return _context.SaveChanges() > 0 ;
        }
    }
}
