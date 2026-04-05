using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmitFormController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        private readonly DocumentService _docService;
        public SubmitFormController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm] int docId)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("Sub"));
            string methodName = MethodOfLogSystem.POST.ToString();
            string roleName = _userService.GetClaimValue(ClaimTypes.Role);
            string uniqueName = _userService.GetClaimValue("unique_name");
            bool existing = ValidateDocumentInDB(docId);
            if (!existing)
            {
                return BadRequest("Failed to upload document");
            }
            string fileName = await _docService.UploadDoc(file, docId, roleName, userId,uniqueName);
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
                return BadRequest("Failed to upload document");

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
        private bool ValidateDocumentInDB(int docId)
        {
            return _context.DocumentTypes.Any(d => d.DocTypeId == docId);
        }
    }
}
