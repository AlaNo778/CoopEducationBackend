using CoopEducation.Controllers;
using CoopEducation.Models;
using CoopEducation.Models.DTO;
using Microsoft.EntityFrameworkCore;
using Supabase.Gotrue;
using System.IO.Compression;
using static CoopEducation.Models.Constant.ConstantVariables;

namespace CoopEducation.Services
{
    public class DocumentService
    {
        private readonly CoopEducationDbContext _context;
        private readonly AllServices _allServices;
        private readonly Supabase.Client _supabaseClient;
        public DocumentService(CoopEducationDbContext context, AllServices allServices,Supabase.Client supabaseClient)
        {
            _context = context;
            _allServices = allServices;
            _supabaseClient = supabaseClient;
        }
        public async Task<Stream> GetDocuments(int docRequest, int userId)
        {
            try
            {
                if (docRequest > 0)
                {
                    string supabaseUrl = GetSupabaseUrl();
                    string supabaseKey = GetSupabaseKey();
                    var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
                    string documentName = GetDocumentName(docRequest);

                    await supabaseClient.InitializeAsync();
                    var bucket = supabaseClient.Storage.From("TemplateDocument");

                    var signedUrl = await bucket.CreateSignedUrl(documentName, 300);
                    signedUrl = signedUrl.TrimEnd('?');

                    using var httpClient = new HttpClient();
                    var fileBytes = await httpClient.GetByteArrayAsync(signedUrl);
                    return new MemoryStream(fileBytes);

                }
                return null!;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"[ERROR] GetDocuments failed: {ex.Message}");
                Console.WriteLine($"[ERROR] StackTrace: {ex.StackTrace}");
                var logs = _allServices.PrepareLog("DocumentService", "", docRequest.ToString(), ex.ToString(), "", userId);
                _allServices.SysApilogs(logs);
                return null!;
            }
        }
        private string GetDocumentName(int docId)
        {
            return _context.DocumentTypes.Where(d => d.DocTypeId == docId).Select(d => d.DocName).FirstOrDefault() ?? "Sc01.pdf";
        }
        public async Task<string> UploadDoc(IFormFile file, int docId, string roleName,int userId,string uniqueName)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return string.Empty;
                }

                var bucket = _supabaseClient.Storage.From(GetbucketName(roleName));
                string documentName = GetDocumentName(docId);

                string folderName = $"{userId}_{uniqueName}";
                string filePath = $"{folderName}/{documentName}";

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                var response = await bucket.Upload(fileBytes, filePath, new Supabase.Storage.FileOptions { Upsert = true });
                if (response != null)
                {
                    SetLogDocDTO logDoc = _allServices.LogDoc(roleName,userId,docId,filePath,null);
                    _allServices.SysDocLogs(logDoc,roleName);
                    return filePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                var logs = _allServices.PrepareLog("DocumentService", "", "", ex.ToString(), NSTools.GetEnumDescription(ResponseCode.Error) ?? "", userId);
                _allServices.SysApilogs(logs);
                return string.Empty;
            }
        }
        private static string GetSupabaseUrl()
        {
            return Convert.ToString(NSTools.GetAppConfig("SUPABASE_URL")) ?? "";
        }
        private static string GetSupabaseKey()
        {
            return Convert.ToString(NSTools.GetAppConfig("SUPABASE_KEY")) ?? "";
        }
        private static string GetbucketName(string roleName)
        {
            return roleName == "student" ? "StudentsSubmittedDocument" : "TeacherSubmittedDocument";
        }
        public async Task<Stream> GetAllDocumentsByRole(string roleName, int userId)
        {
            try
            {
                List<int> docIds = GetDocIdsByRole(roleName);
                if (docIds == null || docIds.Count == 0)
                    return null!;
                
                var bucket = _supabaseClient.Storage.From("TemplateDocument");

                var zipStream = new MemoryStream();

                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
                {
                    using var httpClient = new HttpClient();

                    foreach (var docId in docIds)
                    {
                        string documentName = GetDocumentName(docId);
                        if (string.IsNullOrEmpty(documentName)) continue;

                        try
                        {
                            var signedUrl = await bucket.CreateSignedUrl(documentName, 300);
                            signedUrl = signedUrl.TrimEnd('?');

                            var fileBytes = await httpClient.GetByteArrayAsync(signedUrl);

                            var entry = archive.CreateEntry(documentName, CompressionLevel.Fastest);
                            using var entryStream = entry.Open();
                            await entryStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[WARN] Skip file {documentName}: {ex.Message}");
                        }
                    }
                }
                zipStream.Position = 0;
                return zipStream;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[ERROR] GetDocumentsByRole failed: {ex.Message}");
                var logs = _allServices.PrepareLog("DocumentService", "", roleName.ToString(), ex.ToString(), NSTools.GetEnumDescription(ResponseCode.Error) ?? "", userId);
                _allServices.SysApilogs(logs);
                return null!;
            }
        }
        private List<int> GetDocIdsByRole(string roleName)
        {
            return roleName switch
            {
                "student" => new List<int> { 1, 3, 4, 11, 12, 13, 14, 15, 31},
                "teacher" => new List<int> { 6, 7, 8, 9, 10, 57, 58 },
                "staff" => new List<int> { 1, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 31},
                "admin" => new List<int> { 1, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 31},
                _ => new List<int>()
            };
        }
        public async Task<List<int?>> GetExistDoc(int userId, string roleName)
        {
            if (roleName == "student")
            {
                int studentId = await GetStudentId(userId);

                return await _context.StudentDocuments
                    .Where(x => x.StudentId == studentId)
                    .GroupBy(x => x.DocTypeId)
                    .Select(g => g.OrderByDescending(x => x.UploadedAt).First().DocTypeId)
                    .Select(x => (int?)x)
                    .ToListAsync();
            }

            if (roleName == "teacher")
            {
                int teacherId = await GetteacherId(userId);

                return await _context.TeacherDocuments
                    .Where(x => x.TeacherId == teacherId)
                    .GroupBy(x => x.DocTypeId)
                    .Select(g => g.OrderByDescending(x => x.UploadedAt).First().DocTypeId)
                    .Select(x => (int?)x)
                    .ToListAsync();
            }

            return new List<int?>();
        }
        public async Task<List<int?>> GetDocReportAndThesis(string studentCode)
        {
            int studentId = await _context.Students
                .Where(x => x.StudentCode == studentCode)
                .Select(x => x.StudentId)
                .FirstOrDefaultAsync();

            return await _context.StudentDocuments
                    .Where(x => x.StudentId == studentId)
                    .GroupBy(x => x.DocTypeId)
                    .Select(g => g.OrderByDescending(x => x.UploadedAt).First().DocTypeId)
                    .Select(x => (int?)x)
                    .ToListAsync();
        }
        public async Task<int> GetStudentId(int userId)
        {
            int studentId = await _context.Students
                .Where(x => x.UserId == userId)
                .Select(x => x.StudentId)
                .FirstOrDefaultAsync();

            return studentId;
        }
        public async Task<int> GetteacherId(int userId)
        {
            int teacherId = await _context.Teachers
                .Where(x => x.UserId == userId)
                .Select(x => x.TeacherId)
                .FirstOrDefaultAsync();

            return teacherId;
        }
        public async Task<string> UploadReport(IFormFile file, int docId, string roleName, int userId, string uniqueName,string? studentCode)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return string.Empty;
                }

                var bucket = _supabaseClient.Storage.From("StudentReport");
                string documentName = GetDocumentName(docId);
                string folderName;
                string filePath;
                int? studentUserId = 0;
                int? studentId = 0;

                if (studentCode == null)
                {
                     folderName = $"{userId}_{uniqueName}";
                     filePath = $"{folderName}/{documentName}";
                }
                else
                {
                      studentUserId = await _context.Students
                        .Where(s => s.StudentCode == studentCode)
                        .Select(s => s.UserId)
                        .FirstOrDefaultAsync();

                     folderName = $"{studentUserId}_{studentCode}";
                     filePath = $"{folderName}/{documentName}";
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                var response = await bucket.Upload(fileBytes, filePath, new Supabase.Storage.FileOptions { Upsert = true });
                if (response != null)
                {
                    if (docId == 51 || docId == 55)
                    {
                        int docTypeId = docId == 51 ? 48 : 49;

                        studentId = await _context.Students
                        .Where(s => s.StudentCode == studentCode)
                        .Select(s => s.StudentId)
                        .FirstOrDefaultAsync();

                        var studentDoc = _context.StudentDocuments
                            .Where(x => x.StudentId == studentId &&
                                        x.DocTypeId == docTypeId)
                            .OrderByDescending(x => x.UploadedAt)
                            .FirstOrDefault();

                        if (studentDoc != null)
                        {
                            studentDoc.Approved = true;
                            _context.SaveChanges();
                        }
                    }

                    SetLogDocDTO logDoc = _allServices.LogDoc(roleName, userId, docId, filePath,studentId);
                    _allServices.SysDocLogs(logDoc, roleName);

                    return filePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                var logs = _allServices.PrepareLog("DocumentService", "", "", ex.ToString(), NSTools.GetEnumDescription(ResponseCode.Error) ?? "", userId);
                _allServices.SysApilogs(logs);
                return string.Empty;
            }
        }
        public async Task<string?> GetSignedUrl(int userId, int docId, string uniqueName,string? studentCode)
        {
            var bucket = _supabaseClient.Storage.From("StudentReport");
            string filePath;
            if (studentCode == null)
            {
                 filePath = $"{userId}_{uniqueName}/{GetDocumentName(docId)}";
            }
            else
            {
                int? studentUserId = await _context.Students
                    .Where(s => s.StudentCode == studentCode)
                    .Select(s => s.UserId)
                    .FirstOrDefaultAsync();
                filePath = $"{studentUserId}_{studentCode}/{GetDocumentName(docId)}";
            }
            var signedUrl = await bucket.CreateSignedUrl(filePath, 60);
            signedUrl = signedUrl.TrimEnd('?');
            return signedUrl;
        }
        public async Task<List<DocumentExistDto>> GetexistDocId(int userId)
        {
            int studentId = await _context.Students
                .Where(x => x.UserId == userId)
                .Select(x => x.StudentId)
                .FirstOrDefaultAsync();

            List<DocumentExistDto> exist = _context.StudentDocuments
                .Where(x => x.StudentId == studentId)
                .Select(x => new { x.DocTypeId, x.UploadedAt, x.Approved }) 
                .AsEnumerable()
                .GroupBy(x => x.DocTypeId)
                .Select(g => g.OrderByDescending(x => x.UploadedAt).First())
                .Select(x => new DocumentExistDto
                {
                    DocTypeId = x.DocTypeId,
                    Approved = x.Approved
                })
                .ToList();

            return exist;
        }
        public async Task<List<int>> GetexistReplyDocId(int userId)
        {
            int studentId = _allServices.GetStudentId(userId);

            return await _context.TeacherDocuments
                .Where(x => x.StudentId == studentId &&
                            (x.DocTypeId == 51 || x.DocTypeId == 55))
                .GroupBy(x => x.DocTypeId)
                .Select(g => g.OrderByDescending(x => x.UploadedAt)
                              .Select(x => x.DocTypeId)
                              .FirstOrDefault())
                .ToListAsync();
        }
        public async Task<bool> UpdateFinalStatus(int docId, string studentCode, int userId)
        {
            try
            {
                int studentId = await _context.Students
                    .Where(s => s.StudentCode == studentCode)
                    .Select(s => s.StudentId)
                    .FirstOrDefaultAsync();

                var studentDoc = await _context.StudentDocuments
                    .Where(x => x.StudentId == studentId && x.DocTypeId == docId)
                    .OrderByDescending(x => x.UploadedAt)
                    .FirstOrDefaultAsync();

                if (studentDoc != null)
                {
                    studentDoc.Approved = true;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                var logs = _allServices.PrepareLog("DocumentService", "", docId.ToString(), ex.ToString(), NSTools.GetEnumDescription(ResponseCode.Error) ?? "", userId);
                _allServices.SysApilogs(logs);
                return false;
            }
        }

        }

}
