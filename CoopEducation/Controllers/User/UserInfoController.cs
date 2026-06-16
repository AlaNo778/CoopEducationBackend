using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        public UserInfoController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [Authorize(Roles = "student,teacher,staff,aadmin")]
        [HttpGet]
        public async Task<ResponseMessage<UserInfoDTO>> GetUserInfo()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userId > 0)
            {
                UserInfoDTO? userInfo = await allServices.GetUserInfo(userId, userRole);
                if (userInfo == null)
                {
                    return allServices.WriteResponse<UserInfoDTO>(null, "User information not found.", (NSTools.GetEnumDescription(ResponseCode.NotFound)!).ToString(), true);
                }
                return allServices.WriteResponse<UserInfoDTO>(userInfo, "User information retrieved successfully.", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            }
            return allServices.WriteResponse<UserInfoDTO>(null, "Unauthorized access.", (NSTools.GetEnumDescription(ResponseCode.Unauthorized)!).ToString(), true);
        }
    }
}
