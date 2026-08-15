using CoopEducation.Models;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Supabase.Gotrue;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentInfoController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        private readonly DocumentService _docService;

        public DocumentInfoController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
                _tokenService = tokenService;
                _context = context;
                allServices = new(_context, _tokenService);
                _userService = userService;
                _docService = docService;
        }
        [HttpGet]
        public async Task<List<int?>> GetDocumentinfo([FromQuery]int? tUserId,[FromQuery] string? tRoleName)
        {
            if (tUserId > 0 && tRoleName != null)
            {
                List<int?> listDoc = await _docService.GetExistDoc(null, null, tUserId, tRoleName);
                return listDoc;
            }
            else
            {
                int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
                string roleName = _userService.GetClaimValue(ClaimTypes.Role);
                List<int?> listDoc = await _docService.GetExistDoc(userId, roleName,null,null);
                return listDoc;
            }
        }
    }
}
