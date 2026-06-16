using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllTemplateDocumentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly DocumentService _docService;
        private readonly IUserService _userService;
        public AllTemplateDocumentController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllTemplateDocuments()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string methodName = MethodOfLogSystem.GET.ToString();
            string roleName= _userService.GetClaimValue(ClaimTypes.Role);
            
            var zipStream = await _docService.GetAllDocumentsByRole(roleName, userId);

            if (zipStream == null)
            {
                var errorLog = allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    roleName,
                    "No files",
                    NSTools.GetEnumDescription(ResponseCode.NotFound) ?? "",
                    userId
                );
                allServices.SysApilogs(errorLog);
                return NotFound("Documents not found");
            }

            var successLog = allServices.PrepareLog(
                methodName,
                ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                roleName,
                "Files returned",
                NSTools.GetEnumDescription(ResponseCode.Success) ?? "",
                userId
            );
            allServices.SysApilogs(successLog);
            return File(zipStream, "application/zip", "documents.zip");

        }

    }
}
