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
    public class CompanyListController : ControllerBase
    {
        private readonly AllServices allServices;
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        public CompanyListController(ITokenService tokenService, CoopEducationDbContext context) 
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
        }
        [HttpGet]
        public async Task<ResponseMessage<List<CompanyInfoDTO>>> GetCompanyList()

         {
            var companyInfo = await allServices.GetCompanyInfo();
            return allServices.WriteResponse(companyInfo, "Company info retrieved successfully", (NSTools.GetEnumDescription(ResponseCode.Success)!), false);
        }
    }
}
