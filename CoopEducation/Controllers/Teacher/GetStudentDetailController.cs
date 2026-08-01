using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.Teacher
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetStudentDetailController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public GetStudentDetailController (ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            _allServices = new(_context, _tokenService);
            _userService = userService;
        }
        [HttpGet]
        public async Task<ResponseMessage<AdviseeStudentsDTO>> GetStudent(string studentCode)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string userRole = _userService.GetClaimValue(ClaimTypes.Role);
            if (userRole == "teacher" && userId > 0)
            {
                var prepareData = await _allServices.GetAdviseeStudents(userId, studentCode);
                var student = await _allServices.ConvertListToSingle(prepareData);
                if (student == null)
                {
                    return _allServices.WriteResponse<AdviseeStudentsDTO>(null, "No advisee students found.", (NSTools.GetEnumDescription(ResponseCode.NotFound)!).ToString(), true);
                }
                return _allServices.WriteResponse<AdviseeStudentsDTO>(student, "Advisee students retrieved successfully.", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            }
            return _allServices.WriteResponse<AdviseeStudentsDTO>(null, "Unauthorized access.", (NSTools.GetEnumDescription(ResponseCode.Unauthorized)!).ToString(), true);
        }
    }
}
