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
    public class TeacherInfoController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        public TeacherInfoController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [Authorize(Roles = "teacher")]
        [HttpGet]
        public async Task<ResponseMessage<TeacherInfoDTO>> GetTeacherInfo()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole == "teacher" && userId > 0)
            {
                TeacherInfoDTO? teacherInfo = await allServices.GetTeacherInfo(userId);
                if (teacherInfo == null)
                {
                    return allServices.WriteResponse<TeacherInfoDTO>(null, "Teacher information not found.", (NSTools.GetEnumDescription(ResponseCode.NotFound)!).ToString(), true);
                }
                return allServices.WriteResponse<TeacherInfoDTO>(teacherInfo, "Teacher information retrieved successfully.", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            }
            return allServices.WriteResponse<TeacherInfoDTO>(null, "Unauthorized access.", (NSTools.GetEnumDescription(ResponseCode.Unauthorized)!).ToString(), true);
        }
    }
}
