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
                SameSite = SameSiteMode.Strict,
                Path = "/"
            };
            if(request.RememberMe)
            {
                var refreshToken = _tokenService.GenerateRefreshToken();
                int number = NSTools.IsNumeric(NSTools.GetAppConfig("RefreshTokenExpiryDays")) ? Convert.ToInt16(NSTools.GetAppConfig("RefreshTokenExpiryDays")) : 7;
                var refreshExpiry = DateTime.UtcNow.AddDays(number);
                cookieOptions.Expires = refreshExpiry;
                Response.Cookies.Append("refresh_token", refreshToken, cookieOptions);
                var refreshTokenEntity = new RefreshToken
                {
                    Token = refreshToken,
                    UserId = user.UserId,
                    Expiry = refreshExpiry,
                    CreatedAt = DateTime.UtcNow,
                    Revoked = false
                };
                await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                await _context.SaveChangesAsync();
            }
            else
            {
                var oldRefreshToken = Request.Cookies["refresh_token"];

                if (!string.IsNullOrEmpty(oldRefreshToken))
                {
                    var token = await _context.RefreshTokens
                        .FirstOrDefaultAsync(t => t.Token == oldRefreshToken);

                    if (token != null)
                    {
                        token.Revoked = true;
                    }

                    Response.Cookies.Delete("refresh_token");
                    await _context.SaveChangesAsync();
                }
            }
            int AccessTokenExpiryMinutes = NSTools.IsNumeric(NSTools.GetAppConfig("AccessTokenExpiryMinutes")) ? Convert.ToInt16(NSTools.GetAppConfig("AccessTokenExpiryMinutes")) : 60;
            Response.Cookies.Append("access_token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes),
                Path = "/"
            });
            Response.Cookies.Append("csrf_token", csrfToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });
            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                CsrfToken = csrfToken,
                Role = user.RoleName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes)
            });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(t => t.Token == refreshToken);

                if (token != null)
                {
                    token.Revoked = true;
                    await _context.SaveChangesAsync();
                }
            }
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
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

            Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok(new { accessToken = newAccessToken });
        }
        private async Task<ValidateUserDTO?> ValidateUser(string username,string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive == true);
            if (user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return new ValidateUserDTO
                    {
                        UserId = user.UserId,
                        Username = user.Username,
                        RoleName = user.Role.RoleName
                    };
                }
            }
            return null;
        }
    }
}
