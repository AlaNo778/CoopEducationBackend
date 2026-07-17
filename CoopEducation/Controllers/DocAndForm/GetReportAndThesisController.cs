using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetReportAndThesisController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly DocumentService _docService;
        private readonly IUserService _userService;
        public GetReportAndThesisController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpGet]
        public async Task<IActionResult> GetReportAndThesis(int docId)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            string methodName = MethodOfLogSystem.GET.ToString();
            string roleName = _userService.GetClaimValue(ClaimTypes.Role);
            string uniqueName = _userService.GetClaimValue("unique_name");
            var steamFile = await _docService.GetReportAndThesisDocuments(userId, docId, uniqueName);
            if (steamFile == null || steamFile == Stream.Null || steamFile.Length == 0)
            {
                return NotFound("ไม่พบไฟล์เอกสาร");
            }
            return File(steamFile, "application/pdf");
        }

    }
}
