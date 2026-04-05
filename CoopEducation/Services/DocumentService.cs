using CoopEducation.Controllers;
using CoopEducation.Models;
using CoopEducation.Models.DTO;
using CoopEducation.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using Supabase.Storage;
using System.Reflection;
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
                    //Console.WriteLine(signedUrl);
                    //return new MemoryStream();
                }
                else
                {
                    return null!;
                }
            }
            catch (Exception ex)
            {
                var logs = _allServices.PrepareLog("DocumentService", "", docRequest.ToString(), ex.ToString(), "", userId);
                _allServices.SysApilogs(logs);
                return null!;
            }
        }
        private string GetDocumentName(int docId)
        {
            return _context.DocumentTypes.Where(d => d.DocTypeId == docId).Select(d => d.DocName).FirstOrDefault() ?? "SK01.pdf";
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
                string fileName = $"{userId}_{uniqueName}_{documentName}";
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();
                var response = await bucket.Upload(fileBytes, fileName, new Supabase.Storage.FileOptions { Upsert = true });
                if (response != null)
                {
                    SetLogDocDTO logDoc = _allServices.LogDoc(roleName,userId,docId,fileName);
                    _allServices.SysDocLogs(logDoc);
                    return fileName;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                var logs = _allServices.PrepareLog("DocumentService", "", "", ex.ToString(), "", userId);
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
    }
}
