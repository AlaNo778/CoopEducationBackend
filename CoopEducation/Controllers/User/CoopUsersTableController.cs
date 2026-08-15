using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoopEducation.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoopUsersTableController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public CoopUsersTableController(CoopEducationDbContext context, ITokenService tokenService, AllServices allServices, IUserService userService)
        {
            _context = context;
            _tokenService = tokenService;
            _allServices = allServices;
            _userService = userService;
        }
        [HttpGet]
        public async Task<IActionResult> GetData(int roleId)
        {
            if (roleId == 1)
            {
                var teachers = await _context.Teachers
                    .Join(
                        _context.Users,
                        teacher => teacher.UserId,
                        user => user.UserId,
                        (teacher, user) => new
                        {
                            teacher,
                            user
                        }
                    )
                    .Where(x => x.user.IsActive == true)
                    .Join(
                        _context.Majors,
                        x => x.teacher.Major,
                        major => major.MajorId,
                        (x, major) => new
                        {
                            x.teacher.TeacherId,
                            x.teacher.UserId,
                            x.teacher.FirstName,
                            x.teacher.LastName,
                            x.teacher.Position,
                            x.teacher.Email,
                            x.teacher.Phone,
                            x.teacher.Major,
                            major.MajorId,
                            major.MajorName
                        }
                    )
                    .ToListAsync();

                if (teachers.Any())
                {
                    return Ok(teachers);
                }

                return NotFound("ไม่พบข้อมูลอาจารย์");
            }
            if (roleId == 2)
            {
                var students = await _context.Students
                    .Join(
                        _context.Users,
                        student => student.UserId,
                        user => user.UserId,
                        (student, user) => new
                        {
                            student,
                            user
                        }
                    )
                    .Where(x => x.user.IsActive == true)
                    .Join(
                        _context.Majors,
                        x => x.student.MajorId,
                        major => major.MajorId,
                        (x, major) => new
                        {
                            x.student.StudentId,
                            x.student.UserId,
                            x.student.StudentCode,
                            x.student.FirstName,
                            x.student.LastName,
                            x.student.Email,
                            x.student.Faculty,
                            x.student.Gpax,
                            x.student.TotalCredits,
                            x.student.MajorId,
                            major.MajorName
                        }
                    )
                    .ToListAsync();

                if (students.Any())
                {
                    return Ok(students);
                }

                return NotFound("ไม่พบข้อมูลนักศึกษา");
            }

            return BadRequest("Role ไม่ถูกต้อง");
        }
    }
}
