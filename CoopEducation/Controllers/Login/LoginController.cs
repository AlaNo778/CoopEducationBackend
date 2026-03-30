using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Request;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoopEducation.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly CoopEducationDbContext _context;
        public LoginController(ITokenService tokenService,CoopEducationDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await ValidateUser(request.Username, request.Password);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }
            var accessToken = _tokenService.GenerateAccessToken(user.UserId, user.Username, user.RoleName);
            var csrfToken = _tokenService.GenerateCsrfToken();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            };
            var oldToken = Request.Cookies["refresh_token"];
            if (!string.IsNullOrEmpty(oldToken))
            {
                var existing = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == oldToken);
                if (existing != null) existing.Revoked = true;
            }
            if (request.RememberMe)
            {
                int refreshExpiryDays = int.TryParse(NSTools.GetAppConfig("RefreshTokenExpiryDays"), out var d) ? d : 7;
                var refreshExpiry = DateTime.UtcNow.AddDays(refreshExpiryDays);
                var refreshToken = _tokenService.GenerateRefreshToken();

                Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = refreshExpiry,
                    Path = "/"
                });

                await _context.RefreshTokens.AddAsync(new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.UserId,
                    Expiry = refreshExpiry,
                    CreatedAt = DateTime.UtcNow,
                    Revoked = false
                });
            }
            else
            {
                Response.Cookies.Delete("refresh_token"); // ลบ cookie อย่างเดียว (DB ถูก revoke ไปแล้วด้านบน)
            }
            await _context.SaveChangesAsync();

            int accessTokenExpiryMinutes = int.TryParse(NSTools.GetAppConfig("AccessTokenExpiryMinutes"), out var m) ? m : 60;
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes),
                Path = "/"
            });
            Response.Cookies.Append("csrf_token", csrfToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                CsrfToken = csrfToken,
                Role = user.RoleName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == refreshToken && (t.Revoked == false) && t.Expiry > DateTime.UtcNow);

                if (token != null)
                {
                    token.Revoked = true;
                    await _context.SaveChangesAsync();
                }
            }

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            Response.Cookies.Delete("csrf_token");

            return NoContent();
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var tokenEntity = await _context.RefreshTokens
                .Include(t => t.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(t => t.Token == refreshToken && t.Revoked == false);

            if (tokenEntity == null || tokenEntity.Expiry < DateTime.UtcNow)
                return Unauthorized();

            var newAccessToken = _tokenService.GenerateAccessToken(
                tokenEntity.User.UserId,
                tokenEntity.User.Username,
                tokenEntity.User.Role.RoleName
            );
            int accessTokenExpiryMinutes = int.TryParse(NSTools.GetAppConfig("AccessTokenExpiryMinutes"), out var m) ? m : 60;
            Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(accessTokenExpiryMinutes)
            });

            return Ok(new { accessToken = newAccessToken });
        }
        private async Task<ValidateUserDTO?> ValidateUser(string username,string password)
        {
            var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);

            var dummyHash = "$2a$11$00000000000000000000000000000000000000000000000000000";
            var hash = user?.Password ?? dummyHash;
            var valid = BCrypt.Net.BCrypt.Verify(password, hash);

            if (user == null || !valid) return null;

            return new ValidateUserDTO
            {
                UserId = user.UserId,
                Username = user.Username,
                RoleName = user.Role.RoleName
            };
        }
    }
}
