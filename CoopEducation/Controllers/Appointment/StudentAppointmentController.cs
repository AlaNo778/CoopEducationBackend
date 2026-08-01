using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;

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
    }
}
