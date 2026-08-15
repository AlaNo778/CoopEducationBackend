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
    public class GetViewFileStaffController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly DocumentService _docService;
        private readonly IUserService _userService;
        public GetViewFileStaffController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpGet]
        public async Task<string> GetReportAndThesis(string roleName, int id, int docId)
        {
            var filePath = await _docService.PreviewFile(roleName,id,docId);
            return filePath;
        }
    }
}
