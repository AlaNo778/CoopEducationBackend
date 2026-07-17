using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoopEducation.Controllers.Student
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetStudentIdController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        public GetStudentIdController(CoopEducationDbContext context, ITokenService tokenService, AllServices allServices, IUserService userService)
        {
            _context = context;
            _tokenService = tokenService;
            this.allServices = allServices;
            _userService = userService;
        }
        [HttpGet]
        public async Task<int> GetStudentId()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole == "student" && userId > 0)
            {
                return allServices.GetStudentId(userId);
            }
            return 0;
        }
            
    }
}
