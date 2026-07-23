using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoopEducation.Controllers.Teacher
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAdvisorAssignmentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public StudentAdvisorAssignmentController(CoopEducationDbContext context, ITokenService tokenService, AllServices allServices, IUserService userService)
        {
            _context = context;
            _tokenService = tokenService;
            _allServices = allServices;
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> AssignmentSyudent(AdvisorshipDTO data)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            _allServices.AssignmentStudent(userId, data);
            return Ok(new{message = "Assignment successful"});
        }
    }
}
