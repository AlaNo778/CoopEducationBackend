using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.Teacher
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdviseeStudentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        public AdviseeStudentController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [Authorize(Roles = "teacher")]
        [HttpGet]
        public async Task<ResponseMessage<List<AdviseeStudentsDTO>>> GetAdviseeStudents()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("Sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole == "teacher" && userId > 0)
            {
                List<AdviseeStudentsDTO?> adviseeStudents = await allServices.GetAdviseeStudents(userId);
                if (adviseeStudents == null || adviseeStudents.Count == 0)
                {
                    return allServices.WriteResponse<List<AdviseeStudentsDTO>>(null, "No advisee students found.", (NSTools.GetEnumDescription(ResponseCode.NotFound)!).ToString(), true);
                }
                return allServices.WriteResponse<List<AdviseeStudentsDTO>>(adviseeStudents!, "Advisee students retrieved successfully.", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            }
            return allServices.WriteResponse<List<AdviseeStudentsDTO>>(null, "Unauthorized access.", (NSTools.GetEnumDescription(ResponseCode.Unauthorized)!).ToString(), true);
        }
    }
}
