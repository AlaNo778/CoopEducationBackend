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
                //เหมือนจะต้องเก็บ refresh token ลง database ด้วยนะครับ เพื่อใช้ในการตรวจสอบและยกเลิก token ได้ในอนาคต รอมาแก้
            }
            else
            {
                // ไม่ Remember Me: Session cookie (ปิด browser = หมดอายุ)
                // ไม่ set Expires → เป็น session cookie อัตโนมัติ
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
            return Ok(new LoginResponse
            {
                AccessToken = accessToken,
                CsrfToken = csrfToken,
                Role = user.RoleName,
                ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenExpiryMinutes)
            });
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return NoContent();
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
