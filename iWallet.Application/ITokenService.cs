namespace iWallet.Application
{
    public interface ITokenService
    {
        string GenerateJwtToken(int userId, string email, string role);
        void WriteTokenToCookie(string cookieName , string token, DateTime expiretion);
    }
}
