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
        public SetLogDocDTO LogDoc(string roleName, int userId, int docId, string fileName)
        {
            if (roleName == "student")
            {
                SetLogDocDTO logDoc = new SetLogDocDTO
                {
                    StudentId = Convert.ToInt32(GetStudentId(userId)),
                    DocTypeId = docId,
                    FileName = fileName,
                    PlacementId = Convert.ToInt32(GetPlacementId(userId)),
                    UploadedAt = DateTime.Now,
                };
                return logDoc;
            }
            return new SetLogDocDTO();
        }
        public void SysDocLogs(SetLogDocDTO setLogDocDto)
        {
            StudentDocument docLog = new StudentDocument()
            {
                StudentId = setLogDocDto.StudentId,
                DocTypeId = setLogDocDto.DocTypeId,
                FileName = setLogDocDto.FileName,
                PlacementId = setLogDocDto.PlacementId,
                UploadedAt = setLogDocDto.UploadedAt
            };
            _context.StudentDocuments.Add(docLog);
            _context.SaveChanges();
        }
        private string GetPlacementId(int userId)
        {
            int studentId = GetStudentId(userId);
            return _context.CoopPlacements.Where(c => c.StudentId == studentId).Select(s => s.PlacementId).FirstOrDefault().ToString() ?? "";
        }
        private int GetStudentId(int userId)
        {
            return _context.Students.Where(s => s.UserId == userId).Select(s => s.StudentId).FirstOrDefault();
        }
    }
}
