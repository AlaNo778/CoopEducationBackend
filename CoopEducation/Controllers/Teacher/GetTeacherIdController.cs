using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoopEducation.Controllers.Teacher
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetTeacherIdController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public GetTeacherIdController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            _allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetTeacherId()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(System.Security.Claims.ClaimTypes.Role);
            if (userRole != "teacher" && userRole != "student")
            {
                return Unauthorized("Only teachers and students can access this endpoint.");
            }
            if(userRole == "teacher")
            {
                int teacher = _allServices.GetTeacherId(userId);
                if (teacher < 0)
                {
                    return NotFound("Teacher not found.");
                }
                return Ok(teacher);
            }
            else if(userRole == "student")
            {
                int studentId = _allServices.GetStudentId(userId);
                if (studentId < 0)
                {
                    return NotFound("Student not found");
                }
                var teacherId = await _context.Advisorships
                                        .Where(a => a.StudentId == studentId)
                                        .Select(a => a.TeacherId)
                                        .FirstOrDefaultAsync();
                return Ok(teacherId);
            }
            return BadRequest();
        }

    }
}
