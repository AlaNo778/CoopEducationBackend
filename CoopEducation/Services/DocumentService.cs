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
        public DocumentService(CoopEducationDbContext context, AllServices allServices)
        {
            _context = context;
            _allServices = allServices;
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

                string supabaseUrl = GetSupabaseUrl();
                string supabaseKey = GetSupabaseKey();
                var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
                await supabaseClient.InitializeAsync();
                var bucket = supabaseClient.Storage.From(GetbucketName(roleName));
                string documentName = GetDocumentName(docId);

                string folderName = $"{userId}_{uniqueName}";
                string filePath = $"{folderName}/{documentName}";

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                var response = await bucket.Upload(fileBytes, filePath, new Supabase.Storage.FileOptions { Upsert = true });
                if (response != null)
                {
                    SetLogDocDTO logDoc = _allServices.LogDoc(roleName,userId,docId,filePath);
                    _allServices.SysDocLogs(logDoc);
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
                string supabaseUrl = GetSupabaseUrl();
                string supabaseKey = GetSupabaseKey();
                var supabaseClient = new Supabase.Client(supabaseUrl, supabaseKey);
                await supabaseClient.InitializeAsync();
                var bucket = supabaseClient.Storage.From("TemplateDocument");

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
                "teacher" => new List<int> { 6, 7, 8, 9, 10},
                "staff" => new List<int> { 1, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 31},
                "admin" => new List<int> { 1, 3, 4, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 31 },
                _ => new List<int>()
            };
        }
        public async Task<List<int?>> GetexistDoc(int userId)
        {
            int studentId = await _context.Students
                .Where(x => x.UserId == userId)
                .Select(x => x.StudentId)
                .FirstOrDefaultAsync();

            List<int?> exist = _context.StudentDocuments
                .Where(x => x.StudentId == studentId)
                .Select(x => new { x.DocTypeId, x.UploadedAt })
                .AsEnumerable()
                .GroupBy(x => x.DocTypeId)
                .Select(g => g.OrderByDescending(x => x.UploadedAt).First())
                .Select(x => (int?)x.DocTypeId)
                .Cast<int?>()
                .ToList();

            if (exist.Count == 0)
            {
                return new List<int?>();
            }

            return exist;
        }

    }
}
