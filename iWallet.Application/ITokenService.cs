namespace iWallet.Application
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        void WriteTokenToCookie(string cookieName , string token, DateTime expiretion);
    }
}
