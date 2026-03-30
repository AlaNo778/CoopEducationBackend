using System.IdentityModel.Tokens.Jwt;
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

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal GetUser()
        {
            var token = GetTokenFromCookie();
            if (string.IsNullOrEmpty(token))
                return new ClaimsPrincipal(); 

            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                return new ClaimsPrincipal();

            var jsonToken = handler.ReadJwtToken(token);
            var claims = jsonToken?.Claims ?? Enumerable.Empty<Claim>();

            return new ClaimsPrincipal(new ClaimsIdentity(claims));
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