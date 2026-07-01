using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Response;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

                                     join ad in _context.Advisorships on s.StudentId equals ad.StudentId into sad
                                     from Advisorships in sad.DefaultIfEmpty()

                                     join t in _context.Teachers on Advisorships.TeacherId equals t.TeacherId into ta
                                     from Teacher in ta.DefaultIfEmpty()

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
                                         VillageNo = StudentAddress != null ? StudentAddress.VillageNo : null,
                                         Advisor = Teacher != null ? Teacher.FirstName + " " + Teacher.LastName : null,
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
        public async Task<List<MajorDTO>> GetMajors()
        {
            return await _context.Majors
                .Select(m => new MajorDTO
                {
                    MajorId = m.MajorId,
                    MajorName = m.MajorName
                })
                .ToListAsync();
        }

        public async Task<CoopAndMentorInfoDTO?> GetStudentCoopInfo(int userId)
        {
            var studentCoopInfo = await _context.Students
                    .Where(s => s.UserId == userId)
                    .Select(s => new CoopAndMentorInfoDTO
                    {
                        StudentId = s.StudentId,
                        Coop = s.CoopPlacements
                            .Select(cp => new CoopPlacementDTO
                            {
                                CompanyId = cp.CompanyId,
                                CompanyName = cp.Company.CompanyName,
                                CompanyPhone = cp.Company.Phone,
                                CompanyFax = cp.Company.Fax,
                                CompanyEmail = cp.Company.Email,
                                HrName = cp.Company.HrName,
                                Address = cp.Company.Address,

                                JobTitle = cp.JobTitle,
                                JobDescription = cp.JobDescription,
                                StartDate = cp.StartDate,
                                EndDate = cp.EndDate,
                                AcademicYear = cp.AcademicYear,

                                Mentor = cp.Mentor == null ? null : new MentorDTO
                                {
                                    MentorId = cp.Mentor.MentorId,
                                    FirstName = cp.Mentor.FirstName,
                                    LastName = cp.Mentor.LastName,
                                    Position = cp.Mentor.Position,
                                    Department = cp.Mentor.Department,
                                    MentorPhone = cp.Mentor.Phone,
                                    MentorEmail = cp.Mentor.Email,
                                }
                            })
                            .FirstOrDefault() // null ถ้าไม่มี coop_placement
                    })
                    .FirstOrDefaultAsync(); // null ถ้าไม่เจอ student

                        return studentCoopInfo;
        }
        public async Task<List<CompanyInfoDTO>> GetCompanyInfo()
        {
            return await _context.Companies
                .Select(c => new CompanyInfoDTO
                {
                    CompanyId = c.CompanyId,
                    CompanyName = c.CompanyName
                }).ToListAsync();
        }
        public async Task<(bool success, string message)> UpdateMentorAsync(int userId, UpdateAndAssignMentorDTO mentorDto)
        {
            Mentor mentor = new Mentor
            {
                MentorId = mentorDto.MentorId,
                FirstName = mentorDto.FirstName,
                LastName = mentorDto.LastName,
                Position = mentorDto.Position,
                Department = mentorDto.Department,
                Phone = mentorDto.Phone,
                Email = mentorDto.Email
            };
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return (false, "Student not found.");
            }
            var coopPlacement = await _context.CoopPlacements.FirstOrDefaultAsync(cp => cp.StudentId == student.StudentId);
            if (coopPlacement == null)
            {
                return (false, "Coop placement not found for the student.");
            }
            var existingMentor = await _context.Mentors.FirstOrDefaultAsync(m => m.MentorId == coopPlacement.MentorId);
            if (existingMentor == null)
            {
                return (false, "Mentor not found.");
            }
            existingMentor.FirstName = mentor.FirstName;
            existingMentor.LastName = mentor.LastName;
            existingMentor.Position = mentor.Position;
            existingMentor.Department = mentor.Department;
            existingMentor.Phone = mentor.Phone;
            existingMentor.Email = mentor.Email;
            try
            {
                await _context.SaveChangesAsync();
                return (true, "Mentor updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while updating the mentor: {ex.Message}");
            }
        }
        public async Task<(bool success, string message)> AssignMentor(int userId , UpdateAndAssignMentorDTO mentorDto)
        {
            Mentor mentor = new Mentor
            {
                MentorId = mentorDto.MentorId,
                CompanyId = mentorDto.CompanyId,
                FirstName = mentorDto.FirstName,
                LastName = mentorDto.LastName,
                Position = mentorDto.Position,
                Department = mentorDto.Department,
                Phone = mentorDto.Phone,
                Email = mentorDto.Email
            };
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return (false, "Student not found.");
            }
            var coopPlacement = await _context.CoopPlacements.FirstOrDefaultAsync(cp => cp.StudentId == student.StudentId);
            if (coopPlacement == null)
            {
                return (false, "Coop placement not found for the student.");
            }
            var existingMentor = await _context.Mentors.FirstOrDefaultAsync(m => m.MentorId == coopPlacement.MentorId);
            if (existingMentor == null)
            {
                var newMentor = new Mentor
                {
                    CompanyId = coopPlacement.CompanyId,
                    FirstName = mentor.FirstName,
                    LastName = mentor.LastName,
                    Position = mentor.Position,
                    Department = mentor.Department,
                    Phone = mentor.Phone,
                    Email = mentor.Email
                };
                _context.Mentors.Add(newMentor);
                await _context.SaveChangesAsync();
                coopPlacement.MentorId = newMentor.MentorId;
                await _context.SaveChangesAsync();
                return (true, "Assign mentor successfully");
            }
            return (false, "Mentor already assigned.");

        }
        public async Task<List<MentorDTO>> GetMentorsByCompanyId(int companyId)
        {
            var mentors = await _context.Mentors
                .Where(m => m.CompanyId == companyId)
                .Select(m => new MentorDTO
                {
                    MentorId = m.MentorId,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Position = m.Position,
                    Department = m.Department,
                    MentorPhone = m.Phone,
                    MentorEmail = m.Email
                })
                .ToListAsync();
            return mentors;
        }
        public async Task<(bool success, string message)> AddCompanyAsync(int userId, CompanyDTO companyDto)
        {
            try
            {
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.CompanyName == companyDto.CompanyName);
                if (existingCompany != null)
                {
                    return (false, "Company already exists.");
                }
                var newCompany = new Company
                {
                    CompanyName = companyDto.CompanyName,
                    Phone = companyDto.Phone,
                    Fax = companyDto.Fax,
                    Email = companyDto.Email,
                    HrName = companyDto.HrName,
                    Address = companyDto.Address,
                    CreateAd = DateTime.Now
                };
                _context.Companies.Add(newCompany);
                await _context.SaveChangesAsync();
                return (true, "Company added successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred while adding the company: {ex.Message}");
            }
        }
}}
