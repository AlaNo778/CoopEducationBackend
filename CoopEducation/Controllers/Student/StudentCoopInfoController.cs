using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;
using Microsoft.AspNetCore.Authorization;

namespace CoopEducation.Controllers.Student
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentCoopInfoController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        private readonly IStudentUpdateService _studentUpdateService;

        public StudentCoopInfoController (ITokenService tokenService,CoopEducationDbContext context,IUserService userService,IStudentUpdateService studentUpdateService)
        {
            _tokenService = tokenService;
            _context = context;
            _allServices = new(context, tokenService);
            _userService = userService;
            _studentUpdateService = studentUpdateService;
        }
        [Authorize(Roles = "student")]
        [HttpGet]
        public async Task<ResponseMessage<CoopAndMentorInfoDTO>> GetStudentCoopInfo()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole == "student" && userId > 0)
            {
                CoopAndMentorInfoDTO? studentCoopInfo = await _allServices.GetStudentCoopInfo(userId);
                if (studentCoopInfo?.Coop == null)
                {
                    return _allServices.WriteResponse<CoopAndMentorInfoDTO>(null, "Student cooperate information not found.", (NSTools.GetEnumDescription(ResponseCode.NotFound)!).ToString(), true);
                }
                return _allServices.WriteResponse<CoopAndMentorInfoDTO>(studentCoopInfo, "Student cooperate information retrieved successfully.", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            }
            return _allServices.WriteResponse<CoopAndMentorInfoDTO>(null, "Unauthorized access.", (NSTools.GetEnumDescription(ResponseCode.Unauthorized)!).ToString(), true);
        }
    }
}
