namespace iWallet.Application.Interface
{
    public interface IOtpRepository
    {
        bool CreateOtp(Otp otp);
        bool UpdateOtp(Otp otp);
        string ResendOtp(string userEmail);
    }
}
