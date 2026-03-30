using CoopEducation.Models;
using CoopEducation.Models.DTO;

namespace CoopEducation.Services
{
    public class AllServices
    {
        private readonly CoopEducationDbContext _context;
        private readonly ITokenService _tokenService;
        public AllServices(CoopEducationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        public SetLogDTO PrepareLog(string methodName, string api, string? request, string? response, string statusCode, int createBy)
        {
            return new SetLogDTO()
            {
                ApiEndpoint = api,
                Method = methodName,
                StatusCode = statusCode,
                Request = request,
                Response = response,
                CreateBy = createBy,
            };
        }
        public void SysApilogs(SetLogDTO setLogDto)
        {
            ApiLog apiLog = new ApiLog()
            {
                ApiEndpoint = setLogDto.ApiEndpoint,
                Method = setLogDto.Method,
                StatusCode = setLogDto.StatusCode,
                Request = setLogDto.Request,
                Response = setLogDto.Response,
                CreateBy = setLogDto.CreateBy,
                CreateAt = DateTime.Now
            };
            _context.ApiLogs.Add(apiLog);
            _context.SaveChanges();
        }
    }
}
