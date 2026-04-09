
namespace iWallet.Infrastructure.Implemention
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _configuration = configuration;
            _httpContext = httpContext;
        }
        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.Role, user.Role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credintial = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
               (
               _configuration["JWT:Issure"],
               _configuration["JWT:Audience"],
               claims,
               signingCredentials: credintial,
               expires: DateTime.UtcNow.AddMinutes(15)
               );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void WriteTokenToCookie(string cookieName, string token, DateTime expiretion)
        {
            _httpContext.HttpContext.Response.Cookies.Append(cookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Expires = expiretion,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.Strict
            });
        }
    }
}
