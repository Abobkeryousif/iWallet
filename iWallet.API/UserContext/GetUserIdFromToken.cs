namespace iWallet.API.UserContext
{
    public class GetUserIdFromToken : IGetUserIdFromToken
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserIdFromToken(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public int UserIdFromToken()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userIdClaim = user?.Claims
                .FirstOrDefault(c => c.Type == "sub" || c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                throw new UnauthorizedAccessException("User Id not found or invalid token");

            return userId;
        }
    }
}
