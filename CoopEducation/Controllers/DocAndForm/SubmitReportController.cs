using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;
namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmitReportController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        private readonly DocumentService _docService;
        public SubmitReportController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [Authorize(Roles = "student,teacher")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadReport([FromForm] IFormFile file, [FromForm] int docId, [FromForm] string? studentCode)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string methodName = MethodOfLogSystem.POST.ToString();
            string roleName = _userService.GetClaimValue(ClaimTypes.Role);
            string uniqueName = _userService.GetClaimValue("unique_name");
            bool existing = ValidateReportInDB(docId);
            if (!existing)
            {
                return BadRequest("Failed to upload Report");
            }
            string fileName = await _docService.UploadReport(file, docId, roleName, userId, uniqueName, studentCode);
            if (string.IsNullOrEmpty(fileName))
            {
                var errorLog = allServices.PrepareLog(
                      methodName,
                      ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                      docId.ToString() ?? "",
                      "Upload failed",
                      NSTools.GetEnumDescription(ResponseCode.Error) ?? "",
                      userId
                  );
                allServices.SysApilogs(errorLog);
                return BadRequest("Failed to upload Report");

            }
            else
            {
                var successLog = allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    docId.ToString() ?? "",
                    "Upload successful",
                    NSTools.GetEnumDescription(ResponseCode.Success) ?? "",
                    userId
                );
                allServices.SysApilogs(successLog);
                return Ok(fileName + " uploaded successfully");
            }
        }
        private bool ValidateReportInDB(int docId)
        {
            return _context.DocumentTypes.Any(d => d.DocTypeId == docId);
        }
    }
    
}
