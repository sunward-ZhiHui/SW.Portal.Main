using DevExpress.AspNetCore.Spreadsheet;
using DevExpress.Xpo;
using DocumentViewer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading;

namespace DocumentViewer.Controllers
{
    public class ViewFileController : Controller
    {
        private static HttpClient webClient = new HttpClient();
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public ViewFileController(ILogger<HomeController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(Guid? url)
        {
            @ViewBag.isDownload = "No";
            HttpContext.Session.Remove("Share");
            SpreadsheetDocumentContentFromBytes viewmodel = new SpreadsheetDocumentContentFromBytes();
            var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
            var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
            var fileurl = string.Empty;
            var docShareDoc = _context.DocumentDmsShare.FirstOrDefault(f => f.SessionId == url && (f.IsDeleted == null || f.IsDeleted == false));
            HttpContext.Session.SetString("Share", "isShare");
            @ViewBag.Share = "isShare";
            if (docShareDoc != null)
            {
                var per = "Yes";
                if (docShareDoc.IsExpiry == true)
                {
                    var curDate = DateTime.Now.Date;
                    if (docShareDoc.ExpiryDate != null && curDate <= docShareDoc.ExpiryDate.Value.Date)
                    {
                        per = "Yes";
                    }
                    else
                    {
                        viewmodel.Id = -1;
                        per = "No";
                    }

                }
                if (per == "Yes")
                {
                    var currentDocuments = _context.Documents.Where(w => w.SessionId == docShareDoc.DocSessionId && w.IsLatest == true).FirstOrDefault();
                    if (currentDocuments != null)
                    {
                        if (currentDocuments.IsNewPath == true)
                        {
                            fileurl = fileNewUrl + currentDocuments.FilePath;
                        }
                        else
                        {
                            fileurl = fileOldUrl + currentDocuments.FilePath;
                        }
                        try
                        {
                            viewmodel.Extensions = "";
                            viewmodel.Url = string.IsNullOrEmpty(fileurl) ? "" : fileurl;
                            viewmodel.Id = 1;
                            viewmodel.DocumentId = "1";
                            viewmodel.FileName = currentDocuments.FileName;
                            if (!string.IsNullOrEmpty(fileurl))
                            {
                                string s = viewmodel.Url.Split('.').Last();
                                viewmodel.Extensions = s.ToLower();
                                var uri = new Uri(fileurl);
                                var host = uri.Host;

                                string contentType = currentDocuments.ContentType;
                                if (contentType != null)
                                {
                                    var webResponse = await webClient.GetAsync(new Uri(fileurl));
                                    Stream byteArrayAccessor() => webResponse.Content.ReadAsStream();
                                    viewmodel.DocumentId = Guid.NewGuid().ToString();
                                    viewmodel.ContentAccessorByBytes = byteArrayAccessor;
                                    viewmodel.Type = contentType.Split("/")[0].ToLower();
                                    viewmodel.ContentType = contentType;
                                    System.GC.Collect();
                                    GC.SuppressFinalize(this);
                                }
                                else
                                {
                                    viewmodel.Id = 0;
                                }
                            }
                            else
                            {
                                viewmodel.Id = 0;
                            }
                        }
                        catch (Exception ex)
                        {
                            viewmodel.Id = 0;
                        }
                    }
                }
            }
            return View(viewmodel);
        }
        [HttpPost]
        public ContentResult DxDocRequest()
        {
            return (ContentResult)SpreadsheetRequestProcessor.GetResponse(HttpContext);
        }
    }
}
