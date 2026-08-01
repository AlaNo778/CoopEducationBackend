using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.DocAndForm
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetThesisReplyIdsController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        private readonly DocumentService _docService;

        public GetThesisReplyIdsController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpGet]
        public async Task<List<int>> getThesisId()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            List<int> listDoc = await _docService.GetexistReplyDocId(userId);
            return listDoc;
        }
    }
}
