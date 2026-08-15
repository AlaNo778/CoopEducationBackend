using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;
using Microsoft.EntityFrameworkCore;

namespace CoopEducation.Controllers.Appointment
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentAppointmentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public StudentAppointmentController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            _allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> BookAppointment([FromBody] StudentBookAppointmentDTO dto)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(System.Security.Claims.ClaimTypes.Role);
            if (userRole != "student")  
            {
                return Unauthorized("Only students can book appointments.");
            }
            bool success = await _allServices.BookAppointmentSlotAsync(userId, dto);
            if (!success)
            {
                return BadRequest("Failed to book appointment slot.");
            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> BookingDetail()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(System.Security.Claims.ClaimTypes.Role);

            if (userRole != "student")
            {
                return Unauthorized();
            }

            int studentId = _allServices.GetStudentId(userId);

            var bookingDetail = await _context.SupervisionAppointments
                .Where(s => s.StudentId == studentId && s.CancelledAt == null)
                .Select(s => new BookingDetailDTO
                {
                    AppointmentId = s.AppointmentId,
                    SlotId = s.SlotId,
                    StudentId = s.StudentId,
                    TeacherId = s.TeacherId,
                    AppointmentStatus = s.AppointmentStatus,
                    StudentNote = s.StudentNote,
                    TeacherNote = s.TeacherNote,
                    BookedAt = s.BookedAt,
                    Slot = s.Slot,
                })
                .FirstOrDefaultAsync();

            return Ok(bookingDetail);
        }
    }
}
