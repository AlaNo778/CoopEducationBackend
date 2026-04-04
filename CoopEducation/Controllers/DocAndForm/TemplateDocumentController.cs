using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Request;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateDocumentController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly DocumentService _docService;
        private readonly IUserService _userService;
        public TemplateDocumentController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService,DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpGet]
        public async Task<IActionResult> GetTemplateDocuments([FromQuery] int docId)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("Sub"));
            string methodName = MethodOfLogSystem.GET.ToString();
            var documents = await _docService.GetDocuments(docId, userId);
            if (documents == null)
            {
                var errorLog = allServices.PrepareLog(
                    methodName,
                    ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                    docId.ToString() ?? "",
                    "No file",
                    NSTools.GetEnumDescription(ResponseCode.NotFound) ?? "",
                    userId
                );

                allServices.SysApilogs(errorLog);

                return NotFound("Document not found");
            }
            var successLog = allServices.PrepareLog(
                methodName,
                ControllerContext.ActionDescriptor.AttributeRouteInfo?.Template ?? "",
                docId.ToString() ?? "",
                "File returned",
                NSTools.GetEnumDescription(ResponseCode.Success) ?? "",
                userId
            );
            allServices.SysApilogs(successLog);
            return File(documents, "application/pdf", "documents.pdf");
        }
    }
}
