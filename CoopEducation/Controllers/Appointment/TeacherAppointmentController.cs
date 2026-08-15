using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace CoopEducation.Controllers.Appointment
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherAppointmentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public TeacherAppointmentController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            _allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [HttpPost("create_appointment_slot")]
        public async Task<IActionResult> CreateAppointmentSlot([FromBody] AppointmentSlotDTO appointmentDto)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole != "teacher")
            {
                return Unauthorized("Only teachers can create appointment slots.");
            }
            bool success = await _allServices.CreateAppointmentSlotAsync(userId, appointmentDto);
            if (!success)
            {
                return BadRequest("Failed to create appointment slot.");
            }
            return Ok();
        }
        [HttpGet("get_appointment_slots")]
        public async Task<IActionResult> GetAppointmentSlots(int teacherId)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);

            var appointmentSlots = await _allServices.GetTeacherAvailableSlotsAsync(teacherId);
            return Ok(appointmentSlots);
        }
        [HttpPatch("confirm_booking")]
        public async Task<IActionResult> ConfirmBooking(int appointmentId)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);

            if (userRole != "teacher")
            {
                return Unauthorized("Only teachers can confirm bookings.");
            }

            bool success = await _allServices.ConfirmBookingAsync(appointmentId);
            if (!success)
            {
                return BadRequest("Failed to confirm booking.");
            }
            return Ok();
        }
        [HttpGet("get_appointment_detail")]
        public async Task<IActionResult> GetAppointmentDetail()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);

            if (userRole != "teacher")
                return Unauthorized("Only teachers can get bookings.");

            int teacherId = _allServices.GetTeacherId(userId);

            var data = await (
                from appointment in _context.SupervisionAppointments
                join student in _context.Students
                    on appointment.StudentId equals student.StudentId
                join slot in _context.TeacherAvailableSlots
                    on appointment.SlotId equals slot.SlotId
                where appointment.TeacherId == teacherId
                select new TeacherAppointmentDetailDTO
                {
                    AppointmentId = appointment.AppointmentId,
                    StudentId = appointment.StudentId,
                    StudentName = student.FirstName + " " + student.LastName,
                    StudentNote = appointment.StudentNote,
                    TeacherNote = appointment.TeacherNote,
                    AppointmentStatus = appointment.AppointmentStatus,
                    BookedAt = appointment.BookedAt,
                    SlotId = appointment.SlotId,
                    Slot = slot
                }
            ).ToListAsync();

            return Ok(data);
        }

    }
}
