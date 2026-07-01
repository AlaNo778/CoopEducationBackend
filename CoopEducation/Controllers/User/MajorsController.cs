using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using CoopEducation.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class MajorsController : ControllerBase
    {
        private readonly AllServices allServices;
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        public MajorsController(ITokenService tokenService, CoopEducationDbContext context) 
        {
            _tokenService = tokenService;
            _context = context;
            allServices = new(_context, _tokenService);
        }
        public async Task<ResponseMessage<List<MajorDTO>>> GetMajors()
        {
            var majors = await allServices.GetMajors();
            var response = allServices.WriteResponse<List<MajorDTO>>(majors, "Majors retrieved successfully", (NSTools.GetEnumDescription(ResponseCode.Success)!).ToString(), false);
            return response;
        }
    }
}
