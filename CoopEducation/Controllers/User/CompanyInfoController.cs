using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoopEducation.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyInfoController : ControllerBase
    {
        private readonly AllServices allServices;
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        public CompanyInfoController(ITokenService tokenService, CoopEducationDbContext context) 
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
        }
        public async Task<List<CompanyInfoDTO>> GetCompanyInfo()
        {
            var companyInfo = await allServices.GetCompanyInfo();
            return companyInfo;
        }
    }
}
