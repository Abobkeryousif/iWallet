
namespace iWallet.Infrastructure.Implemention
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContext)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContext;
        }
        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credintial = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
               (
               _configuration["JWT:Issuer"],
               _configuration["JWT:Audience"],
               claims,
               signingCredentials: credintial,
               expires: DateTime.UtcNow.AddMinutes(15)
               );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public void WriteTokenToCookie(string cookieName, string token, DateTime expiretion)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Expires = expiretion,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
        }
    }
}
