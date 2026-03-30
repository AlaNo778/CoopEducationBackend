using CoopEducation.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CoopEducation.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(int userId, string username, string roleName);
        string GenerateCsrfToken();
        string GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
    public class TokenService : ITokenService
    {
        public string GenerateAccessToken(int userId, string username, string roleName)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(NSTools.GetAppConfig("Key")));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            int number = NSTools.IsNumeric(NSTools.GetAppConfig("AccessTokenExpiryMinutes")) ? Convert.ToInt16(NSTools.GetAppConfig("AccessTokenExpiryMinutes")) : 60;
            var expiry = DateTime.UtcNow.AddMinutes(number);

            var token = new JwtSecurityToken(
                issuer: NSTools.GetAppConfig("Issuer"),
                audience: NSTools.GetAppConfig("Audience"),
                claims: claims,
                expires: expiry,
                signingCredentials: new SigningCredentials(
                    key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateCsrfToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToHexString(bytes).ToLower();
        }
        public string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(bytes);
        }
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(NSTools.GetAppConfig("Key"));
            try
            {
                return handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = NSTools.GetAppConfig("Issuer"),
                    ValidateAudience = true,
                    ValidAudience = NSTools.GetAppConfig("Audience"),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
