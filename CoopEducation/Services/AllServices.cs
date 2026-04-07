using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;

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
        public ResponseMessage<T> WriteResponse<T>(T? data, string? messaage, string? code, bool isError)
        {
            return new ResponseMessage<T>
            {
                code = code,
                message = messaage,
                isError = isError,
                data = data
            };
        }
        public async Task<StudentInfoDTO?> GetStudentInfo(int userId)
        {
            var studentInfo = await (from s in _context.Students
                                     join m in _context.Majors on s.MajorId equals m.MajorId into sm
                                     from major in sm.DefaultIfEmpty()

                                     join c in _context.StudentContacts on s.StudentId equals c.StudentId into sc
                                     from StudentContact in sc.DefaultIfEmpty()

                                     join a in _context.StudentAddresses on s.StudentId equals a.StudentId into sa
                                     from StudentAddress in sa.DefaultIfEmpty()

                                     where s.UserId == userId
                                     select new StudentInfoDTO
                                     {
                                         StudentId = s.StudentId,
                                         StudentCode = s.StudentCode,
                                         FirstName = s.FirstName,
                                         LastName = s.LastName,
                                         Email = s.Email,
                                         Faculty = s.Faculty,
                                         Gpax = s.Gpax,
                                         TotalCredits = s.TotalCredits,
                                         MajorName = major != null ? major.MajorName : null,
                                         Facebook = StudentContact != null ? StudentContact.Facebook : null,
                                         LineId = StudentContact != null ? StudentContact.LineId : null,
                                         PhoneHome = StudentContact != null ? StudentContact.PhoneHome : null,
                                         PhoneMobile = StudentContact != null ? StudentContact.PhoneMobile : null,
                                         Alley = StudentAddress != null ? StudentAddress.Alley : null,
                                         District = StudentAddress != null ? StudentAddress.District : null,
                                         HouseNo = StudentAddress != null ? StudentAddress.HouseNo : null,
                                         Postcode = StudentAddress != null ? StudentAddress.Postcode : null,
                                         Province = StudentAddress != null ? StudentAddress.Province : null,
                                         Road = StudentAddress != null ? StudentAddress.Road : null,
                                         SubDistrict = StudentAddress != null ? StudentAddress.SubDistrict : null,
                                         VillageNo = StudentAddress != null ? StudentAddress.VillageNo : null
                                     }).FirstOrDefaultAsync();
            return studentInfo;
        }
        public async Task<List<AdviseeStudentsDTO?>> GetAdviseeStudents(int userId)
        {
            var adviseeStudents = await (from t in _context.Teachers
                                         join a in _context.Advisorships on t.TeacherId equals a.TeacherId into ta
                                         from adviseStudent in ta.DefaultIfEmpty()

                                         join s in _context.Students on adviseStudent.StudentId equals s.StudentId into ads
                                         from student in ads.DefaultIfEmpty()

                                         join m in _context.Majors on student.MajorId equals m.MajorId into am
                                         from major in am.DefaultIfEmpty()

                                         join c in _context.StudentContacts on student.StudentId equals c.StudentId into sc
                                         from StudentContact in sc.DefaultIfEmpty()

                                         join ad in _context.StudentAddresses on student.StudentId equals ad.StudentId into sa
                                         from StudentAddress in sa.DefaultIfEmpty()

                                         join cp in _context.CoopPlacements on student.StudentId equals cp.StudentId into scp
                                         from CoopPlacement in scp.DefaultIfEmpty()

                                         join mtr in _context.Mentors on CoopPlacement.MentorId equals mtr.MentorId into cm
                                         from Mentor in cm.DefaultIfEmpty()

                                         join cn in _context.Companies on CoopPlacement.CompanyId equals cn.CompanyId into ccn
                                         from Company in ccn.DefaultIfEmpty()

                                         where t.UserId == userId

                                         select new AdviseeStudentsDTO
                                         {
                                             FirstName = student.FirstName,
                                             LastName = student.LastName,
                                             StudentCode = student.StudentCode,
                                             Email = student.Email,
                                             Faculty = student.Faculty,
                                             Gpax = student.Gpax,
                                             TotalCredits = student.TotalCredits,
                                             MajorName = major != null ? major.MajorName : null,
                                             PhoneHome = StudentContact != null ? StudentContact.PhoneHome : null,
                                             PhoneMobile = StudentContact != null ? StudentContact.PhoneMobile : null,
                                             Facebook = StudentContact != null ? StudentContact.Facebook : null,
                                             LineId = StudentContact != null ? StudentContact.LineId : null,
                                             MentorFirstName = Mentor.FirstName,
                                             MentorLastName = Mentor.LastName,
                                             Position = Mentor.Position,
                                             Department = Mentor.Department,
                                             Phone = Mentor.Phone,
                                             MentorEmail = Mentor.Email,
                                             CompanyName = Company.CompanyName,
                                             JobTitle = CoopPlacement.JobTitle,
                                             JobDescription = CoopPlacement.JobDescription,
                                             StartDate = CoopPlacement.StartDate,
                                             EndDate = CoopPlacement.EndDate,
                                             AcademicYear = CoopPlacement.AcademicYear,
                                         }).ToListAsync();


            return adviseeStudents;
        }
        public async Task<TeacherInfoDTO?> GetTeacherInfo(int userId)
        {
            var teacherInfo = await (from t in _context.Teachers
                                     where t.UserId == userId
                                     select new TeacherInfoDTO
                                     {
                                         TeacherId = t.TeacherId,
                                         FirstName = t.FirstName,
                                         LastName = t.LastName,
                                         Position = t.Position,
                                         Email = t.Email,
                                         Phone = t.Phone
                                     }).FirstOrDefaultAsync();
            return teacherInfo;
        }
        public async Task<UserInfoDTO?> GetUserInfo(int userId, string roleName)
        {
            if (roleName == "student")
            {
                var userInfo = await (from s in _context.Students
                                      where s.UserId == userId
                                      select new UserInfoDTO
                                      {
                                          FullName = s.FirstName + " " + s.LastName,
                                          RoleName = roleName
                                      }).FirstOrDefaultAsync();
                return userInfo;

            }
            else if (roleName == "teacher")
            {
                var userInfo = await (from t in _context.Teachers
                                      where t.UserId == userId
                                      select new UserInfoDTO
                                      {
                                          FullName = t.FirstName + " " + t.LastName,
                                          RoleName = roleName
                                      }).FirstOrDefaultAsync();
                return userInfo;
            }
            else if (roleName == "staff")
            {
                var userInfo = await (from a in _context.Staffs 
                                      where a.UserId == userId
                                      select new UserInfoDTO
                                      {
                                          FullName = a.FirstName + " " + a.LastName,
                                          RoleName = roleName
                                      }).FirstOrDefaultAsync();
                return userInfo;
            }
            else if (roleName == "admin")
            {
                var userInfo = new UserInfoDTO
                {
                    FullName = "เจ้าหน้าที่ดูแลระบบ",
                    RoleName = roleName
                };
                return userInfo;
            }
            return null;

        }
    }
}
