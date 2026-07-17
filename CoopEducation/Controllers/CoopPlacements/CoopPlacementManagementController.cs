using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.CoopPlacements
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoopPlacementManagementController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices _allServices;
        private readonly IUserService _userService;
        public CoopPlacementManagementController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            _allServices = new(context, tokenService);
            _userService = userService;
        }
        [HttpPost]
        public async Task<IActionResult> AssignPlacement([FromBody] AssignCompanyDTO assignCompanyDTO)
        {
            var methodName = nameof(AssignPlacement);
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            try
            {
                var (success, message) = await _allServices.AssignCompanyToStudentAsync(userId, assignCompanyDTO);
                var log = _allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    userId.ToString(),
                    message,
                    NSTools.GetEnumDescription(success ? ResponseCode.Success : ResponseCode.Incorrect) ?? "",
                    userId
                );
                _allServices.SysApilogs(log);
                return success
                    ? Ok(new { isError = false, message })
                    : BadRequest(new { isError = true, message });
            }
            catch (Exception ex)
            {
                var errorLog = _allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    userId.ToString(),
                    ex.Message,
                    NSTools.GetEnumDescription(ResponseCode.Error) ?? "",
                    userId
                );
                _allServices.SysApilogs(errorLog);
                return StatusCode(500, new { isError = true, message = "An error occurred while assigning the company to the student." });
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdatePlacement([FromBody] AssignCompanyDTO data)
        {
            var methodName = nameof(UpdatePlacement);
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            try
            {
                var (success, message) = await _allServices.UpdateStudentCoopPlacementAsync(userId, data);
                var log = _allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    userId.ToString(),
                    message,
                    NSTools.GetEnumDescription(success ? ResponseCode.Success : ResponseCode.Incorrect) ?? "",
                    userId
                );
                _allServices.SysApilogs(log);
                return success
                    ? Ok(new { isError = false, message })
                    : BadRequest(new { isError = true, message });
            }
            catch (Exception ex)
            {
                var errorLog = _allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    userId.ToString(),
                    ex.Message,
                    NSTools.GetEnumDescription(ResponseCode.Error) ?? "",
                    userId
                );
                _allServices.SysApilogs(errorLog);
                return StatusCode(500, new { isError = true, message = "An error occurred while updating the student's coop placement." });
            }
        }
    }
}
