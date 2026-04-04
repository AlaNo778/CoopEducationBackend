using CoopEducation.Controllers;
using CoopEducation.Models;
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
        public async Task<Stream> GetDocuments(int docRequest,int userId)
        {
            try
            {
                if (docRequest > 0)
                {
                    string supabaseUrl = Convert.ToString(NSTools.GetAppConfig("SUPABASE_URL"));
                    string supabaseKey = Convert.ToString(NSTools.GetAppConfig("SUPABASE_KEY"));
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
                var logs = _allServices.PrepareLog("DocumentService","",docRequest.ToString(),ex.ToString(),"",userId);
                _allServices.SysApilogs(logs);
                return null!;
            }
        }
        private string GetDocumentName(int docId)
        {
            return _context.DocumentTypes.Where(d => d.DocTypeId == docId).Select(d => d.DocName).FirstOrDefault() ?? "SK01.pdf";
        }
    }
}
