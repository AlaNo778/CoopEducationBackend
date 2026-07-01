using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.Company
{
    [Route("api/[controller]")]
    [ApiController]
    public class MentorsController : ControllerBase
    {
        private readonly AllServices allServices;
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        public MentorsController(ITokenService tokenService, CoopEducationDbContext context)
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
        }
        [HttpGet]
        public async Task<ResponseMessage<List<MentorDTO>>> GetMentors(int companyId)
        {
            var mentors = await allServices.GetMentorsByCompanyId(companyId);
            return allServices.WriteResponse(mentors, "Mentors retrieved successfully", (NSTools.GetEnumDescription(ResponseCode.Success)!), false);
        }
    }
}
