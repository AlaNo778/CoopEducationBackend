using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.Student
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentInfoController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        public StudentInfoController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [Authorize(Roles = "student")]
        [HttpGet]
        public async Task<ResponseMessage<StudentInfoDTO>> GetStudentInfo()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("Sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole == "student" && userId > 0)
            {
                StudentInfoDTO? studentInfo = await allServices.GetStudentInfo(userId);
                if (studentInfo == null)
                {
                    return allServices.WriteResponse<StudentInfoDTO>(null, "Student information not found.",(NSTools.GetEnumDescription(ResponseCode.NotFound)!).ToString(), true);
                }
                return allServices.WriteResponse<StudentInfoDTO>(studentInfo, "Student information retrieved successfully.", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            }
            return allServices.WriteResponse<StudentInfoDTO>(null, "Unauthorized access.", (NSTools.GetEnumDescription(ResponseCode.Unauthorized)!).ToString(), true);
        }
    }
}
