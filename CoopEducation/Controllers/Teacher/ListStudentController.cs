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
    public class ListStudentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public ListStudentController(CoopEducationDbContext context, ITokenService tokenService, AllServices allServices, IUserService userService)
        {
            _context = context;
            _tokenService = tokenService;
            _allServices = allServices;
            _userService = userService;
        }
        [HttpGet]
        public async Task<List<StudentListDTO>> GetListStudent()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            var listStudent = await _allServices.GetListStudentPrepareAssignment(userId);
            return listStudent;
        }
    }
}
