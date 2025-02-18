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
            @ViewBag.isExpired = "No";
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
                        var curDate = DateTime.Now.Date;
                        bool? IsExpiryDate = false;
                        viewmodel.Id = 1;
                        viewmodel.DocumentId = "1";
                        viewmodel.FileName = currentDocuments.FileName;
                        currentDocuments.ExpiryDate = currentDocuments.ExpiryDate;
                        if (currentDocuments.ExpiryDate != null && currentDocuments.ExpiryDate.Value.Date <= curDate)
                        {
                            IsExpiryDate = true;
                            viewmodel.ExpiryDate = currentDocuments.ExpiryDate;
                            viewmodel.IsExpiryDate = true;
                            @ViewBag.isExpired = "Yes";
                        }
                        if (IsExpiryDate == false)
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
                                        var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                                        var webResponse = await webClient.GetAsync(new Uri(fileurl));
                                        var streamData = webResponse.Content.ReadAsStream();
                                        if (Extension == "msg")
                                        {
                                            viewmodel.Type = Extension;
                                            viewmodel.PlainTextBytes = OutLookMailDocuments(streamData, Extension);
                                        }
                                        else
                                        {
                                            viewmodel.Type = contentType.Split("/")[0].ToLower();
                                            Stream byteArrayAccessor() => streamData;
                                            viewmodel.ContentAccessorByBytes = byteArrayAccessor;
                                        }

                                        viewmodel.DocumentId = Guid.NewGuid().ToString();
                                        viewmodel.ContentType = contentType;
                                        //System.GC.Collect();
                                        //GC.SuppressFinalize(this);
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
            }
            return View(viewmodel);
        }
        [HttpPost]
        public ContentResult DxDocRequest()
        {
            return (ContentResult)SpreadsheetRequestProcessor.GetResponse(HttpContext);
        }
        private string OutLookMailDocuments(Stream stream, string extension)
        {
            string plainTextBytes = string.Empty;
            if (extension == "msg")
            {
                using (var msg = new MsgReader.Outlook.Storage.Message(stream))
                {
                    plainTextBytes = msg.BodyHtml;
                    var attachments = msg.Attachments;
                    if (attachments != null && attachments.Count > 0)
                    {
                        attachments.ForEach(a =>
                        {
                            var attachment = a as MsgReader.Outlook.Storage.Message.Attachment;
                            if (attachment.ContentId != null && attachment.IsInline == true)
                            {
                                var contentId = "cid:" + attachment.ContentId;
                                if (plainTextBytes.Contains(contentId))
                                {
                                    var contentType = "image/" + attachment.FileName.Split('.').Last();
                                    var base64String = Convert.ToBase64String(attachment.Data, 0, attachment.Data.Length);
                                    var filebaseString = "data:" + contentType + ";base64," + base64String;
                                    plainTextBytes = plainTextBytes.Replace(contentId, filebaseString);
                                }
                            }
                        });
                    }
                }
            }
            if (extension == "eml")
            {
                var eml = MsgReader.Mime.Message.Load(stream);
                /* if (eml.TextBody != null)
                 {
                     var textBody = System.Text.Encoding.UTF8.GetString(eml.TextBody.Body);
                 }*/

                if (eml.HtmlBody != null)
                {
                    plainTextBytes = System.Text.Encoding.UTF8.GetString(eml.HtmlBody.Body);
                }
                var attachments = eml.Attachments;
                if (attachments != null && attachments.Count > 0)
                {
                    attachments.ToList().ForEach(attachment =>
                    {
                        if (attachment.ContentId != null && attachment.IsInline == true)
                        {
                            var contentId = "cid:" + attachment.ContentId;
                            if (plainTextBytes.Contains(contentId))
                            {
                                var contentType = "image/" + attachment.FileName.Split('.').Last();
                                var base64String = Convert.ToBase64String(attachment.Body, 0, attachment.Body.Length);
                                var filebaseString = "data:" + contentType + ";base64," + base64String;
                                plainTextBytes = plainTextBytes.Replace(contentId, filebaseString);
                            }
                        }
                    });
                }
            }
            return plainTextBytes;
        }
    }
}
