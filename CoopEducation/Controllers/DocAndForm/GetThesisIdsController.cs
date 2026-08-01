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
    public class GetThesisIdsController : ControllerBase
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly AllServices allServices;
        private readonly IUserService _userService;
        private readonly DocumentService _docService;

        public GetThesisIdsController(ITokenService tokenService, CoopEducationDbContext context, IUserService userService, DocumentService docService)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
            _userService = userService;
            _docService = docService;
        }
        [HttpGet]
        public async Task<List<DocumentExistDto>> getThesisId()
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            List<DocumentExistDto> listDoc = await _docService.GetexistDocId(userId);
            return listDoc;
        }
        [HttpPost("update_approve")]
        public async Task<IActionResult> updateFinalStatus([FromQuery] int docId, [FromQuery] string studentCode)
        {
            int userId = Convert.ToInt32(_userService.GetClaimValue("sub"));
            bool result = await _docService.UpdateFinalStatus(docId, studentCode, userId);
            if (result)
            {
                return Ok(new { message = "Status updated successfully" });
            }
            else
            {
                return BadRequest(new { message = "Failed to update status" });
            }
        }
    }
}
