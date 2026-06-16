using System.Security.Claims;

namespace CoopEducation.Services
{
    public interface IUserService
    {
        ClaimsPrincipal GetUser();
        string GetClaimValue(string claimType);
    }

    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;

        public UserService(IHttpContextAccessor httpContextAccessor, ITokenService tokenService)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }

        public ClaimsPrincipal GetUser()
        {
            var token = GetTokenFromCookie();
            if (string.IsNullOrEmpty(token))
                return new ClaimsPrincipal();

            var principal = _tokenService.ValidateToken(token);
            return principal ?? new ClaimsPrincipal();
        }

        public string GetClaimValue(string claimType)
        {
            var user = GetUser();
            return user?.FindFirst(claimType)?.Value ?? string.Empty;
        }

        private string GetTokenFromCookie()
        {
            return _httpContextAccessor?.HttpContext?.Request.Cookies["access_token"]
                   ?? string.Empty;
        }
    }
}