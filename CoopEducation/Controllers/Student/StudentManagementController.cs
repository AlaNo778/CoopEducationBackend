using CoopEducation.Dtos;
using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentManagementController : ControllerBase
{
    private readonly CoopEducationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly AllServices _allServices;
    private readonly IUserService _userService;
    private readonly IStudentUpdateService _studentUpdateService;

    public StudentManagementController(
        ITokenService tokenService,
        CoopEducationDbContext context,
        IUserService userService,
        IStudentUpdateService studentUpdateService)
    {
        _tokenService = tokenService;
        _context = context;
        _allServices = new(context, tokenService);
        _userService = userService;
        _studentUpdateService = studentUpdateService;
    }

    [Authorize(Roles = "student")]
    [HttpPut("update-student")]
    public async Task<IActionResult> UpdateStudent([FromBody] UpdateStudentDto dto)
    {
        const string methodName = nameof(UpdateStudent);
        int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));

        try
        {
            var (success, message) = await _studentUpdateService.UpdateStudentAsync(userId, dto);

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
            return StatusCode(500, new { isError = true, message = "An unexpected error occurred." });
        }
    }
}