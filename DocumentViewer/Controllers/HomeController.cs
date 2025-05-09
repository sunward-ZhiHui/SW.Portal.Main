using DocumentViewer.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using DevExpress.AspNetCore.Spreadsheet;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Export;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Policy;
using System.Net.Mime;
using DocumentViewer.Helpers;
using Microsoft.AspNetCore.Http.Extensions;
using DocumentViewer.EntityModels;
using Microsoft.AspNetCore.Http;
using DocumentApi.Models;
using System.Threading;
using DevExpress.Xpo;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using MsgReader.Outlook;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using System.IO.Compression;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Office.Drawing;
using DevExpress.Data.Filtering.Helpers;
using static DevExpress.Utils.HashCodeHelper.Primitives;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using System.Buffers.Text;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;
using Azure.Core;
using DevExpress.Web.Office;
using DevExpress.Export.Xl;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.qrcode;
using DevExpress.XtraSpreadsheet.Utils;

namespace DocumentViewer.Controllers
{
    public class HomeController : Controller
    {

        private static HttpClient webClient = new HttpClient();
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, AppDbContext context, IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<IActionResult> Index(string url, bool? IsHistory)
        {
            var userId = HttpContext.Session.GetString("user_id");
            Guid? sesId = null;
            if (!string.IsNullOrEmpty(url))
            {
                bool isValid = Guid.TryParse(url, out _);
                if (isValid == true)
                {
                    sesId = new Guid(url);
                }
            }
            bool isMobile = IsMobileDevice(HttpContext, userId, sesId);
            var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
            var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
            var fileurl = string.Empty; var pathurl = string.Empty;
            HttpContext.Session.Remove("fileName");
            HttpContext.Session.Remove("invalid");
            HttpContext.Session.Remove("isDownload");
            HttpContext.Session.Remove("isView");
            HttpContext.Session.Remove("fileUrl"); HttpContext.Session.Remove("DocumentId");
            @ViewBag.isDownload = "No";
            @ViewBag.IsHistory = "Yes";
            @ViewBag.isUrl = "isUrl";
            @ViewBag.isFile = "No";
            IsHistory = IsHistory == true ? true : false;
            //DocumentManager.CloseAllDocuments();
            if (userId != null)
            {
                SpreadsheetDocumentContentFromBytes viewmodel = new SpreadsheetDocumentContentFromBytes();

                viewmodel.IsUserAgent = isMobile;
                @ViewBag.isMobile = isMobile == true ? "Yes" : "No";
                if (!string.IsNullOrEmpty(url))
                {
                    var sessionId = new Guid(url);
                    var currentDocuments = _context.Documents.Where(w => w.UniqueSessionId == sessionId).FirstOrDefault();
                    if (currentDocuments != null)
                    {
                        if (currentDocuments.SourceFrom != "Email")
                        {
                            if (IsHistory == false)
                            {
                                var latestcurrentDocuments = _context.Documents.Where(w => w.SessionId == currentDocuments.SessionId && w.IsLatest == true).FirstOrDefault();
                                if (latestcurrentDocuments != null)
                                {
                                    @ViewBag.IsHistoryDoc = "No";
                                    currentDocuments = latestcurrentDocuments;
                                }
                                else
                                {
                                    viewmodel.Id = 0;
                                    return View(viewmodel);
                                }
                            }
                        }
                        viewmodel.IsLatest = currentDocuments.IsLatest == true ? true : false;
                        HttpContext.Session.SetString("DocumentId", currentDocuments.DocumentId.ToString());
                        var ipirApp = _context.IpirApp.Where(w => w.SessionID == currentDocuments.SessionId).FirstOrDefault();
                        var curDate = DateTime.Now.Date;
                        bool? IsExpiryDate = false;
                        currentDocuments.ExpiryDate = currentDocuments.ExpiryDate;
                        if (currentDocuments.ExpiryDate != null && currentDocuments.ExpiryDate.Value.Date <= curDate)
                        {
                            IsExpiryDate = true;
                            viewmodel.ExpiryDate = currentDocuments.ExpiryDate;
                            viewmodel.IsExpiryDate = true;
                            @ViewBag.isExpired = "Yes";
                        }
                        HttpContext.Session.SetString("fileName", currentDocuments.FileName);
                        long userIds = Int64.Parse(HttpContext.Session.GetString("user_id"));
                        if (!string.IsNullOrEmpty(currentDocuments.FilePath))
                        {
                            if (currentDocuments.SourceFrom == "FileProfile")
                            {
                                var query = from oal in _context.OpenAccessUserLink
                                            join oau in _context.OpenAccessUser on oal.OpenAccessUserId equals oau.OpenAccessUserId
                                            where oal.UserId == userIds && oau.AccessType == "DMSAccess" && oal.IsDmsAccess == true
                                            select oal;
                                var res = query.ToList();
                                if (res != null && res.Count() > 0)
                                {
                                    viewmodel.IsRead = true; viewmodel.IsDownload = true;
                                    HttpContext.Session.SetString("isDownload", "Yes");
                                    HttpContext.Session.SetString("isView", "Yes");
                                    @ViewBag.isDownload = "Yes";
                                }
                                else
                                {
                                    if (ipirApp != null)
                                    {
                                        HttpContext.Session.SetString("isDownload", "Yes");
                                        HttpContext.Session.SetString("isView", "Yes");
                                        @ViewBag.isDownload = "Yes";
                                        viewmodel.IsRead = true; viewmodel.IsDownload = true;
                                    }
                                    else
                                    {
                                        var documentsModel = GetAllSelectedFilePermissionAsync(currentDocuments);
                                        if (documentsModel != null)
                                        {
                                            viewmodel.IsRead = documentsModel.FirstOrDefault()?.DocumentPermissionData.IsRead == true ? true : false;
                                            viewmodel.IsDownload = documentsModel.FirstOrDefault()?.DocumentPermissionData.IsEdit == true ? true : false;
                                            if (userIds == documentsModel.FirstOrDefault()?.UploadedByUserId)
                                            {
                                                viewmodel.IsRead = true; viewmodel.IsDownload = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else if (currentDocuments.SourceFrom == "Email")
                            {
                                var per = GetEmailFilePermissionAsync(currentDocuments);
                                viewmodel.IsRead = per.IsRead; viewmodel.IsDownload = per.IsDownload;
                            }
                            else
                            {
                                viewmodel.IsRead = true; viewmodel.IsDownload = true;
                                @ViewBag.isDownload = "Yes";
                                HttpContext.Session.SetString("isDownload", "Yes");
                                HttpContext.Session.SetString("isView", "Yes");
                            }

                            if (currentDocuments.IsNewPath == true)
                            {
                                pathurl = _configuration["DocumentsUrl:NewFileLivePath"] + @"\\" + currentDocuments.FilePath;
                                fileurl = fileNewUrl + currentDocuments.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
                                var paths = currentDocuments.FilePath.Replace(@"\", @"/");
                                @ViewBag.Url = fileNewUrl + paths;
                            }
                            else
                            {
                                fileurl = fileOldUrl + currentDocuments.FilePath;
                                pathurl = _configuration["DocumentsUrl:OldFileLivePath"] + @"\\" + currentDocuments.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
                                var paths = currentDocuments.FilePath.Replace(@"\", @"/");
                                @ViewBag.Url = fileOldUrl + paths;
                            }
                        }
                        try
                        {
                            viewmodel.Extensions = "";
                            viewmodel.Url = string.IsNullOrEmpty(fileurl) ? "" : fileurl;
                            viewmodel.Id = 1;
                            viewmodel.DocumentId = "1";
                            viewmodel.UniqueId = currentDocuments.DocumentId;
                            viewmodel.FileName = currentDocuments.FileName;
                            viewmodel.FileNameWithOutExtension = RemoveAfterLastDot(currentDocuments.FileName);
                            if (!string.IsNullOrEmpty(fileurl))
                            {
                                string s = viewmodel.Url.Split('.').Last();
                                viewmodel.Extensions = s.ToLower();
                                var uri = new Uri(fileurl);
                                var host = uri.Host;
                                string? contentType = currentDocuments?.ContentType;
                                viewmodel.ContentType = contentType;
                                if (contentType != null)
                                {
                                    var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                                    @ViewBag.FileExtension = Extension;
                                    if (Extension == "doc" || Extension == "docx")
                                    {
                                        if (System.IO.File.Exists(pathurl))
                                        {
                                            viewmodel.PathUrl = pathurl;
                                            if (Extension == "msg" || Extension == "eml")
                                            {
                                                viewmodel.Type = Extension;
                                            }
                                            else
                                            {
                                                viewmodel.Type = contentType.Split("/")[0].ToLower();
                                            }
                                            @ViewBag.isFile = "Yes";
                                        }
                                        else
                                        {
                                            viewmodel.Id = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (System.IO.File.Exists(pathurl))
                                        {
                                            viewmodel.PathUrl = pathurl;
                                            var streamData = await ConvertFileToMemoryStreamAsync(pathurl, null);

                                            if (Extension == "msg" || Extension == "eml")
                                            {
                                                viewmodel.Type = Extension;
                                                viewmodel.PlainTextBytes = OutLookMailDocuments(streamData, Extension);
                                            }
                                            else
                                            {
                                                viewmodel.Type = contentType.Split("/")[0].ToLower();

                                                // Read stream into a byte array to avoid memory leaks
                                                byte[] fileBytes;
                                                using (var memoryStream = new MemoryStream())
                                                {
                                                    await streamData.CopyToAsync(memoryStream);
                                                    fileBytes = memoryStream.ToArray();
                                                }

                                                viewmodel.ContentAccessorByBytes = () => fileBytes;
                                            }
                                            viewmodel.DocumentId = Guid.NewGuid().ToString();
                                            @ViewBag.isFile = "Yes";
                                            viewmodel.ContentType = contentType;
                                        }
                                        else
                                        {
                                            viewmodel.Id = 0;
                                        }
                                    }

                                    return View(viewmodel);
                                }
                                else
                                {
                                    viewmodel.Id = 0;
                                    return View(viewmodel);
                                }
                            }
                            else
                            {
                                viewmodel.Id = 0;
                                return View(viewmodel);
                            }
                        }
                        catch (Exception ex)
                        {
                            viewmodel.Id = 0;
                            return View(viewmodel);
                        }


                    }
                    else
                    {
                        viewmodel.Id = 0;
                        return View(viewmodel);
                    }
                }
                else
                {
                    viewmodel.Id = 0;
                    return View(viewmodel);
                }
            }
            else
            {
                var loginurl = "login?url=" + url;
                if (IsHistory == true)
                {
                    loginurl += "IsHistory=" + true;
                }
                return Redirect(loginurl);
            }
        }
        private bool IsMobileDevice(HttpContext contexts, string? userId, Guid? sessId)
        {
            if (contexts.Request.Headers.TryGetValue("User-Agent", out var userAgent))
            {
                string userAgentString = userAgent.ToString().ToLower();
                long? userIds = Convert.ToInt64(userId);
                var status = userAgentString.Contains("iphone") ||
                       userAgentString.Contains("ipad") ||
                       userAgentString.Contains("ipod") ||
                       userAgentString.Contains("android");
                if (userIds > 0)
                {
                    var DocumentViewer = new DocumentViewers
                    {
                        UserId = userIds,
                        Description = userAgentString,
                        AddedDate = DateTime.Now,
                        SessionId = sessId,
                        IsMobile = status,
                    };
                    _context.Add(DocumentViewer);
                    _context.SaveChanges();
                }

                return status;
            }
            return false;
        }
        string RemoveAfterLastDot(string text)
        {
            int lastIndex = text.LastIndexOf(".");
            return lastIndex > 0 ? text.Substring(0, lastIndex) : text;
        }
        [HttpPost("GetDocumentsVersionTrace")]
        public string GetDocumentsVersionTrace(long? documentId)
        {
            var html = string.Empty;
            //var documentId = (long?)Convert.ToDouble(HttpContext.Session.GetString("DocumentId"));
            if (documentId > 0)
            {
                var currentDocuments = _context.Documents.Where(w => w.DocumentId == documentId).FirstOrDefault();
                if (currentDocuments != null)
                {
                    var userId = (long?)Convert.ToDouble(HttpContext.Session.GetString("user_id"));
                    var query = from oal in _context.DocumentsVersionTrace
                                join oau in _context.ApplicationUser on oal.UserId equals oau.UserId
                                where oal.UserId == userId && oal.DocumentId == documentId
                                select new DocumentsVersionTrace
                                {
                                    DocumentsVersionTraceId = oal.DocumentsVersionTraceId,
                                    UserId = oal.UserId,
                                    DocumentId = oal.DocumentId,
                                    SessionId = oal.SessionId,
                                    UpdateDateTime = oal.UpdateDateTime,
                                    UserName = oau.UserName,
                                    Description = oal.Description,
                                    VersionNo = oal.VersionNo,
                                };
                    var list = query.ToList();
                    if (list != null && list.Count() > 0)
                    {
                        list.OrderByDescending(o => o.DocumentsVersionTraceId).ToList().ForEach(s =>
                        {
                            html += "<li>";
                            html += "<span style=\"font-Weight:bold;\">" + s.UserName + "</span><br>";
                            html += "<span style=\"font-Weight:bold;\">Version:</span><span>" + s.VersionNo + "</span>";
                            html += "<span class=\"event-date\">" + s.UpdateDateTime?.ToString("dd-MMM-yyyy hh.mm tt") + "</span>";
                            html += "<p>" + s.Description + "</p>";
                            html += "</li>";
                        });
                    }
                }
            }
            return html;
        }

        [HttpPost("ExportUrl")]
        public async Task<IActionResult> ExportUrl(IFormFile file, long? id, string? isVersion)
        {
            var currentDocuments = _context.Documents.Where(w => w.DocumentId == id).FirstOrDefault();
            if (currentDocuments != null)
            {
                string pathurl = string.Empty;
                if (currentDocuments.IsNewPath == true)
                {
                    pathurl = _configuration["DocumentsUrl:NewFileLivePath"] + @"\\" + currentDocuments.FilePath;
                }
                else
                {
                    pathurl = _configuration["DocumentsUrl:OldFileLivePath"] + @"\\" + currentDocuments.FilePath;
                }
                if (isVersion == "Yes")
                {
                    if (System.IO.File.Exists(pathurl))
                    {
                        var FileProfileType = _context.FileProfileType.Where(w => w.FileProfileTypeId == currentDocuments.FilterProfileTypeId).FirstOrDefault();
                        var serverPaths = _configuration["DocumentsUrl:NewFileLivePath"] + @"\Documents\" + FileProfileType?.SessionId + @"\";
                        if (!System.IO.Directory.Exists(serverPaths))
                        {
                            System.IO.Directory.CreateDirectory(serverPaths);
                        }
                        var sesid = Guid.NewGuid();
                        var ext = currentDocuments.FileName.Split(".").Last();
                        var filepaths = serverPaths + sesid + "." + ext;
                        long length = new FileInfo(pathurl).Length;
                        System.IO.File.Copy(pathurl, filepaths);
                        var pathsName = @"Documents\" + FileProfileType?.SessionId + @"\" + sesid + "." + ext;
                        VersionDataData(currentDocuments, pathsName, length);
                    }
                }
                if (!string.IsNullOrEmpty(pathurl))
                {
                    byte[] buffer = new byte[4096]; // 4KB buffer

                    using (var inputStream = file.OpenReadStream()) // Read file stream directly
                    using (var outputStream = new FileStream(pathurl, FileMode.Create, FileAccess.Write, FileShare.None, buffer.Length, true))
                    {
                        int bytesRead;
                        while ((bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                        {
                            await outputStream.WriteAsync(buffer, 0, bytesRead);
                        }
                    }
                }
                saveUpdateUserData();
            }
            return Ok();
        }
        void VersionDataData(Documents documents, string? newfilePath, long length)
        {
            long userId = Int64.Parse(HttpContext.Session.GetString("user_id"));
            var currentDocuments = _context.Documents.Where(w => w.DocumentId == documents.DocumentId).FirstOrDefault();
            var DocumentParentId = _context.Documents.Where(w => w.SessionId == documents.SessionId && w.IsLatest == true).FirstOrDefault()?.DocumentParentId;
            if (DocumentParentId > 0)
            {

            }
            else
            {
                DocumentParentId = _context.Documents.Where(w => w.SessionId == documents.SessionId && w.IsLatest == false).FirstOrDefault()?.DocumentParentId;
            }
            if (currentDocuments != null)
            {
                var Documents = new Documents
                {
                    FileName = documents.FileName,
                    ContentType = documents.ContentType,
                    FileSize = length,
                    UploadDate = DateTime.Now,
                    SessionId = documents.SessionId,
                    IsSpecialFile = false,
                    IsTemp = false,
                    StatusCodeId = documents.StatusCodeId,
                    Description = documents.Description,
                    AddedByUserId = userId,
                    AddedDate = DateTime.Now,
                    ModifiedByUserId = userId,
                    ModifiedDate = DateTime.Now,
                    FilterProfileTypeId = documents.FilterProfileTypeId,
                    ProfileNo = documents.ProfileNo,
                    TableName = documents.TableName,
                    DocumentParentId = DocumentParentId,
                    IsLatest = false,
                    ExpiryDate = documents.ExpiryDate,
                    IsNewPath = true,
                    FilePath = newfilePath,
                    SourceFrom = documents.SourceFrom,
                    UniqueSessionId = Guid.NewGuid(),
                };
                _context.Add(Documents);
                _context.SaveChanges();

                if (DocumentParentId > 0)
                {
                    currentDocuments.DocumentParentId = DocumentParentId;
                }
                else
                {
                    currentDocuments.DocumentParentId = Documents.DocumentId;
                }
                currentDocuments.ModifiedDate = DateTime.Now;
                currentDocuments.ModifiedByUserId = userId;
                currentDocuments.FileIndex = null;
                _context.SaveChanges();
                var prevDoc = _context.Documents.Where(w => w.SessionId == documents.SessionId && w.DocumentParentId > 0).ToList();
                if (prevDoc != null && prevDoc.Count() > 0)
                {
                    int i = 1;
                    prevDoc.ForEach(f =>
                    {
                        var appWikiReleaseDocquerys = string.Format("Update Documents Set FileIndex='{0}' Where DocumentId in" + '(' + "{1}" + ')', i, f.DocumentId);
                        _context.Database.ExecuteSqlRaw(appWikiReleaseDocquerys);
                        _context.SaveChanges();
                        i++;
                    });
                }
            }
        }
        public IActionResult RibbonDownloadXlsx(SpreadsheetClientState spreadsheetState)
        {
            var spreadsheet = SpreadsheetRequestProcessor.GetSpreadsheetFromState(spreadsheetState);

            using (MemoryStream stream = new MemoryStream())
            {
                spreadsheet.SaveCopy(stream, DocumentFormat.Xlsx);
                stream.Position = 0; // Reset position for reading

                const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = HttpContext.Session.GetString("fileName") ?? "default.xlsx";

                return File(stream.ToArray(), XlsxContentType, fileName);
            }
        }
        [HttpPost]
        public IActionResult SaveComments(string? name, string? VersionNo)
        {
            var userId = (long?)Convert.ToDouble(HttpContext.Session.GetString("user_id"));
            var documentId = (long?)Convert.ToDouble(HttpContext.Session.GetString("DocumentId"));
            if (documentId > 0)
            {
                var currentDocuments = _context.Documents.Where(w => w.DocumentId == documentId).FirstOrDefault();
                if (currentDocuments != null)
                {
                    var DocumentsTrace = new DocumentsVersionTrace
                    {
                        UserId = userId,
                        Description = name,
                        DocumentId = currentDocuments.DocumentId,
                        SessionId = currentDocuments.SessionId,
                        UpdateDateTime = DateTime.Now,
                        VersionNo = VersionNo,
                    };
                    _context.Add(DocumentsTrace);
                    _context.SaveChanges();
                    return Ok("success");
                }
            }
            return Ok();
        }
        [HttpPost]
        public void RibbonSaveToFile(SpreadsheetClientState spreadsheetState, long? id, string? isVersion)
        {
            var currentDocuments = _context.Documents.Where(w => w.DocumentId == id).FirstOrDefault();
            if (currentDocuments != null)
            {
                var ext = currentDocuments.FileName.Split(".").Last().ToLower();
                string pathurl = string.Empty;
                if (currentDocuments.IsNewPath == true)
                {
                    pathurl = _configuration["DocumentsUrl:NewFileLivePath"] + @"\\" + currentDocuments.FilePath;
                }
                else
                {
                    pathurl = _configuration["DocumentsUrl:OldFileLivePath"] + @"\\" + currentDocuments.FilePath;
                }
                if (isVersion == "Yes")
                {
                    if (System.IO.File.Exists(pathurl))
                    {
                        var FileProfileType = _context.FileProfileType.Where(w => w.FileProfileTypeId == currentDocuments.FilterProfileTypeId).FirstOrDefault();
                        var serverPaths = _configuration["DocumentsUrl:NewFileLivePath"] + @"\Documents\" + FileProfileType?.SessionId + @"\";
                        if (!System.IO.Directory.Exists(serverPaths))
                        {
                            System.IO.Directory.CreateDirectory(serverPaths);
                        }
                        var sesid = Guid.NewGuid();

                        var filepaths = serverPaths + sesid + "." + ext;
                        long length = new FileInfo(pathurl).Length;
                        System.IO.File.Copy(pathurl, filepaths);
                        var pathsName = @"Documents\" + FileProfileType?.SessionId + @"\" + sesid + "." + ext;
                        VersionDataData(currentDocuments, pathsName, length);
                    }
                }

                var spreadsheet = SpreadsheetRequestProcessor.GetSpreadsheetFromState(spreadsheetState);
                string documentId = spreadsheet.DocumentId;
                byte[] documentContent = spreadsheet.SaveCopy(ext == "xls" ? DocumentFormat.Xls : DocumentFormat.Xlsx);
                using (var fs = new FileStream(pathurl, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(documentContent, 0, documentContent.Length);
                }
                //spreadsheet.Save();
                saveUpdateUserData();
            }
        }
        void saveUpdateUserData()
        {
            var documentId = (long?)Convert.ToDouble(HttpContext.Session.GetString("DocumentId"));
            if (documentId > 0)
            {
                var userId = (long?)Convert.ToDouble(HttpContext.Session.GetString("user_id"));
                var currentDocuments = _context.Documents.Where(w => w.DocumentId == documentId).FirstOrDefault();
                if (currentDocuments != null)
                {
                    var DocumentsTrace = new DocumentsTrace
                    {
                        CurrentUserId = userId,
                        PrevUserId = currentDocuments.ModifiedByUserId,
                        DocumentId = currentDocuments.DocumentId,
                        SessionId = currentDocuments.SessionId,
                        UpdateDateTime = DateTime.Now,
                    };
                    _context.Add(DocumentsTrace);
                    _context.SaveChanges();
                    currentDocuments.ModifiedByUserId = userId;
                    currentDocuments.ModifiedDate = DateTime.Now;

                    _context.SaveChanges();
                }
            }
        }
        public IActionResult RibbonDownloadXls(SpreadsheetClientState spreadsheetState)
        {

            var spreadsheet = SpreadsheetRequestProcessor.GetSpreadsheetFromState(spreadsheetState);

            using (MemoryStream stream = new MemoryStream())
            {
                spreadsheet.SaveCopy(stream, DocumentFormat.Xlsx);
                stream.Position = 0; // Reset position for reading

                const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = HttpContext.Session.GetString("fileName") ?? "default.xlsx";

                return File(stream.ToArray(), XlsxContentType, fileName);
            }
        }
        [HttpGet("DownloadFromUrl")]
        public async Task<IActionResult> DownloadFromUrlAsync(string? url, bool? IsHistoryDoc, string? isDownload)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
                    var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
                    var fileurl = string.Empty;
                    var sessionId = new Guid(url);
                    var currentDocuments = _context.Documents.Where(w => w.UniqueSessionId == sessionId).FirstOrDefault();
                    bool? isLatestDoc = true;
                    if (currentDocuments != null)
                    {
                        if (currentDocuments.SourceFrom != "Email")
                        {
                            if (IsHistoryDoc == false)
                            {
                                var latestcurrentDocuments = _context.Documents.Where(w => w.SessionId == currentDocuments.SessionId && w.IsLatest == true).FirstOrDefault();
                                if (latestcurrentDocuments != null)
                                {
                                    isLatestDoc = true;
                                    currentDocuments = latestcurrentDocuments;
                                }
                                else
                                {
                                    isLatestDoc = false;
                                }
                            }
                        }
                        if (isLatestDoc == true)
                        {
                            var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                            if (currentDocuments.IsNewPath == true)
                            {
                                fileurl = _configuration["DocumentsUrl:NewFileLivePath"] + @"\\" + currentDocuments.FilePath;
                            }
                            else
                            {
                                fileurl = _configuration["DocumentsUrl:OldFileLivePath"] + @"\\" + currentDocuments.FilePath;
                            }
                            if (Extension == "msg")
                            {
                                currentDocuments.ContentType = "application/octet-stream";
                            }
                            var IsAllowWaterMarks = _context.FileProfileType.FirstOrDefault(f => f.FileProfileTypeId == currentDocuments.FilterProfileTypeId)?.IsAllowWaterMark;
                            string? isPdf = null;
                            if (IsAllowWaterMarks == true && Extension == "pdf")
                            {
                                isPdf = Extension;
                            }
                            var stream = await ConvertFileToMemoryStreamAsync(fileurl, isPdf);

                            return new FileStreamResult(stream, currentDocuments.ContentType)
                            {
                                FileDownloadName = currentDocuments.FileName
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok("file is not exist");
        }
        private byte[] AddWatermark(Stream stream, BaseFont bf)
        {
            // Ensure the stream is seekable
            if (!stream.CanSeek)
            {
                MemoryStream tempStream = new MemoryStream();
                stream.CopyTo(tempStream);
                stream = tempStream;
                stream.Position = 0;
            }

            using (var ms = new MemoryStream()) // Let MemoryStream handle resizing
            {
                using (var reader = new PdfReader(stream))
                using (var stamper = new PdfStamper(reader, ms))
                {
                    int pageCount = reader.NumberOfPages;
                    for (int i = 1; i <= pageCount; i++)
                    {
                        var content = stamper.GetOverContent(i);
                        // AddWaterMarks(content, "uncontrolled", bf, 70, 35,
                        //new BaseColor(255, 0, 0), reader.GetPageSizeWithRotation(i));
                        AddWaterMarks(content, "uncontrolled", bf, 70, 45, BaseColor.RED, reader.GetPageSizeWithRotation(i));
                    }
                } // PdfStamper & PdfReader disposed automatically

                return ms.ToArray();
            }
        }
        async Task<Stream> ConvertFileToMemoryStreamAsync(string filePath, string? Extension)
        {
            if (Extension == "pdf")
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
                    var byteArray = AddWatermark(memoryStream, bfTimes);
                    Stream stream = new MemoryStream(byteArray);
                    return stream;
                }
            }
            else
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await fileStream.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream; // Caller is responsible for disposing
                }
            }
        }
        [HttpGet("Maildownloadfromurl")]
        public async Task<IActionResult> Maildownloadfromurl(string? url, bool? IsHistoryDoc)
        {
            try
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
                    var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
                    var fileurl = string.Empty;
                    var sessionId = new Guid(url);
                    var currentDocuments = _context.Documents.Where(w => w.UniqueSessionId == sessionId).FirstOrDefault();
                    bool? isLatestDoc = true;
                    if (currentDocuments != null)
                    {
                        if (currentDocuments.SourceFrom != "Email")
                        {
                            if (IsHistoryDoc == false)
                            {
                                var latestcurrentDocuments = _context.Documents.Where(w => w.SessionId == currentDocuments.SessionId && w.IsLatest == true).FirstOrDefault();
                                if (latestcurrentDocuments != null)
                                {
                                    isLatestDoc = true;
                                    currentDocuments = latestcurrentDocuments;
                                }
                                else
                                {
                                    isLatestDoc = false;
                                }
                            }
                        }
                        if (isLatestDoc == true)
                        {
                            var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                            var filnames = currentDocuments.FileName != null ? currentDocuments.FileName?.TrimEnd('.') : "ZipFile";
                            if (currentDocuments.IsNewPath == true)
                            {
                                fileurl = _configuration["DocumentsUrl:NewFileLivePath"] + @"\\" + currentDocuments.FilePath;
                            }
                            else
                            {
                                fileurl = _configuration["DocumentsUrl:OldFileLivePath"] + @"\\" + currentDocuments.FilePath;
                            }
                            if (Extension == "msg")
                            {
                                currentDocuments.ContentType = "application/octet-stream";
                            }

                            var stream = await ConvertFileToMemoryStreamAsync(fileurl, null);
                            if (stream != null)
                            {
                                var streamDatas = GetOutLookMailDocuments(stream, Extension);

                                if (streamDatas != null)
                                {
                                    return File(streamDatas, "application/x-zip-compressed", filnames + ".zip");
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Ok();
        }
        [HttpPost]
        public ContentResult DxDocRequest()
        {
            if (Request.Method != "POST") // Blocks GET requests that trigger automatically
            {
                return Content("Blocked: No automatic requests allowed.");
            }

            return (ContentResult)SpreadsheetRequestProcessor.GetResponse(HttpContext);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = "10" });
        }
        private List<DocumentsModel> GetAllSelectedFilePermissionAsync(Documents currentDocuments)
        {
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            long userId = Int64.Parse(HttpContext.Session.GetString("user_id"));
            var documentPermission = GetDocumentPermissionByRoll();
            DocumentsModel documentsModels = new DocumentsModel();
            var sessionId = currentDocuments?.SessionId;
            var fileProfileId = currentDocuments?.FilterProfileTypeId;
            if (currentDocuments.DocumentId > 0)
            {
                var DocumentUserRoles = _context.DocumentUserRole.Where(w => w.FileProfileTypeId == fileProfileId && w.DocumentId == currentDocuments.DocumentId).ToList();
                var counts = DocumentUserRoles.Count();
                if (counts > 0)
                {
                    var userExits = DocumentUserRoles.Where(w => w.UserId == userId).FirstOrDefault();
                    if (userExits != null)
                    {
                        var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)userExits.RoleId).FirstOrDefault();
                        documentsModels.DocumentPermissionData = permissionData;
                    }
                    else
                    {
                        var DocumentUserRolesFile = _context.DocumentUserRole.Where(w => w.FileProfileTypeId == fileProfileId && w.DocumentId == null).ToList();
                        if (DocumentUserRolesFile.Count > 0)
                        {
                            var userExitsf = DocumentUserRolesFile.Where(w => w.UserId == userId).FirstOrDefault();
                            if (userExitsf != null)
                            {
                                var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)userExitsf.RoleId).FirstOrDefault();
                                documentsModels.DocumentPermissionData = permissionData;
                            }
                            else
                            {
                                documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };

                            }
                        }
                        else
                        {
                            documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };
                        }
                    }
                }
                else
                {
                    var DocumentUserRolesFile = _context.DocumentUserRole.Where(w => w.FileProfileTypeId == fileProfileId && w.DocumentId == null).ToList();
                    if (DocumentUserRolesFile.Count > 0)
                    {
                        var userExitsf = DocumentUserRolesFile.Where(w => w.UserId == userId).FirstOrDefault();
                        if (userExitsf != null)
                        {
                            var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)userExitsf.RoleId).FirstOrDefault();
                            documentsModels.DocumentPermissionData = permissionData;
                        }
                        else
                        {
                            documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };
                        }
                    }
                    else
                    {
                        documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = true, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = true, IsCopy = true, IsCreateFolder = true, IsEdit = true, IsMove = true, IsShare = true, IsFileDelete = true };
                    }
                }
            }
            /*var documents = _context.Documents.Where(w => w.IsLatest == true && w.SessionId == sessionId && w.SourceFrom == "FileProfile").ToList();
            if (documents != null)
            {
                var roleItemsList = _context.DocumentUserRole.Where(w => w.FileProfileTypeId == fileProfileId).ToList();
                documents.ForEach(s =>
                {
                    var documentPermission = GetDocumentPermissionByRoll();
                    var setAccess = roleItemsList;
                    DocumentsModel documentsModels = new DocumentsModel();
                    documentsModels.UploadedByUserId = s.AddedByUserId;
                    documentsModels.DocumentID = s.DocumentId;
                    documentsModels.FileName = s.FileName;
                    HttpContext.Session.SetString("fileUrl", s.FileName);
                    if (setAccess.Count > 0)
                    {
                        var roleDocItem = setAccess.FirstOrDefault(u => u.DocumentId == s.DocumentId);
                        if (roleDocItem != null)
                        {
                            var roleItem = setAccess.FirstOrDefault(u => u.UserId == userId && u.DocumentId == s.DocumentId);
                            if (roleItem != null)
                            {
                                var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)roleItem.RoleId).FirstOrDefault();
                                documentsModels.DocumentPermissionData = permissionData;
                            }
                            else
                            {
                                documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };
                            }
                        }
                        else
                        {
                            var filprofilepermission = setAccess.FirstOrDefault(u => u.FileProfileTypeId == s.FilterProfileTypeId && u.DocumentId == null && u.UserId == userId);
                            if (filprofilepermission != null)
                            {
                                var permissionData = documentPermission.Where(z => z.DocumentRoleID == (int)filprofilepermission.RoleId).FirstOrDefault();
                                documentsModels.DocumentPermissionData = permissionData;
                            }
                            else
                            {
                                documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = true, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = true, IsCopy = true, IsCreateFolder = true, IsEdit = true, IsMove = true, IsShare = true, IsFileDelete = true };
                            }
                        }
                    }
                    else
                    {
                        documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = false,IsEdit=false };
                    }

                    if (documentsModels.DocumentPermissionData != null)
                    {
                        if (documentsModels.DocumentPermissionData.IsRead == true)
                        {
                            documentsModel.Add(documentsModels);
                        }
                        else if (documentsModels.DocumentPermissionData.IsRead == false)
                        {

                        }
                    }
                    else
                    {
                        documentsModel.Add(documentsModels);
                    }
                });
                *//*var roleItems = roleItemsList.Where(w => w.FileProfileTypeId == fileProfileId).ToList();
                if (roleItems.Count > 0)
                {
                    var roleItem = roleItems.FirstOrDefault(u => u.UserId == userId);
                    if (roleItem != null)
                    {
                    }
                    else
                    {
                        if (documentsModel.Count > 0)
                        {
                            documentsModel.ForEach(p =>
                            {
                                p.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = false, IsRename = false };

                            });
                        }
                    }
                }*/
                documentsModel.Add(documentsModels);
                var IsRead = documentsModel.FirstOrDefault()?.DocumentPermissionData.IsRead == true ? "Yes" : "No";
                var isDownload = documentsModel.FirstOrDefault()?.DocumentPermissionData.IsEdit == true ? "Yes" : "No";
                if (userId == documentsModel.FirstOrDefault()?.UploadedByUserId)
                {
                    IsRead = "Yes"; isDownload = "Yes";
                }
                @ViewBag.isDownload = isDownload;
                HttpContext.Session.SetString("isDownload", isDownload);
                HttpContext.Session.SetString("isView", IsRead);

            //}
            return documentsModel;
        }
        private List<DocumentPermissionModel> GetDocumentPermissionByRoll()
        {
            var documentPermission = _context.DocumentPermission.ToList();
            List<DocumentPermissionModel> documentPermissionModel = new List<DocumentPermissionModel>();
            documentPermission.ForEach(s =>
            {
                DocumentPermissionModel documentPermissionModels = new DocumentPermissionModel
                {
                    DocumentPermissionID = s.DocumentPermissionId,
                    DocumentID = s.DocumentId,
                    DocumentRoleID = s.DocumentRoleId,
                    IsRead = s.IsRead,
                    IsCreateFolder = s.IsCreateFolder,
                    IsCreateDocument = s.IsCreateDocument.GetValueOrDefault(false),
                    IsSetAlert = s.IsSetAlert,
                    IsEditIndex = s.IsEditIndex,
                    IsRename = s.IsRename,
                    IsUpdateDocument = s.IsUpdateDocument,
                    IsCopy = s.IsCopy,
                    IsMove = s.IsMove,
                    IsDelete = s.IsDelete,
                    IsRelationship = s.IsRelationship,
                    IsListVersion = s.IsListVersion,
                    IsInvitation = s.IsInvitation,
                    IsSendEmail = s.IsSendEmail,
                    IsDiscussion = s.IsDiscussion,
                    IsAccessControl = s.IsAccessControl,
                    IsAuditTrail = s.IsAuditTrail,
                    IsRequired = s.IsRequired,
                    IsEdit = s.IsEdit,
                    IsFileDelete = s.IsFileDelete,
                    IsGrantAdminPermission = s.IsGrantAdminPermission,
                    IsDocumentAccess = s.IsDocumentAccess,
                    IsCreateTask = s.IsCreateTask,
                    IsEnableProfileTypeInfo = s.IsEnableProfileTypeInfo,
                    IsCloseDocument = s.IsCloseDocument,
                    IsShare = s.IsShare,


                };
                documentPermissionModel.Add(documentPermissionModels);
            });
            return documentPermissionModel.OrderByDescending(a => a.DocumentPermissionID).ToList();
        }


        private PermissionModel GetEmailFilePermissionAsync(Documents documents)
        {
            PermissionModel permissionModel = new PermissionModel();
            long userId = Int64.Parse(HttpContext.Session.GetString("user_id"));
            string mode = documents.SourceFrom;
            var docsessionId = documents?.SessionId;


            var IsRead = "No";

            if (mode == "Email")
            {

                var query = from oal in _context.OpenAccessUserLink
                            join oau in _context.OpenAccessUser on oal.OpenAccessUserId equals oau.OpenAccessUserId
                            where oal.UserId == userId && oau.AccessType == "EmailAccess"
                            select oal;
                var res = query.ToList();
                if (res != null && res.Count() > 0)
                {
                    permissionModel.IsRead = true; permissionModel.IsDownload = true;
                    @ViewBag.isDownload = "Yes";
                    HttpContext.Session.SetString("isDownload", "Yes");
                    HttpContext.Session.SetString("isView", "Yes");
                }
                else
                {
                    var emailConversationlst = _context.EmailConversations.Where(w => w.SessionId == docsessionId).FirstOrDefault();
                    if (emailConversationlst != null)
                    {
                        var topicId = emailConversationlst.TopicId;
                        if (string.IsNullOrEmpty(emailConversationlst.UserType) || emailConversationlst.UserType == "Users")
                        {
                            var plst = _context.EmailConversationParticipant.Where(x => x.TopicId == topicId && x.UserId == userId).FirstOrDefault();

                            if (plst != null)
                            {
                                permissionModel.IsRead = true; permissionModel.IsDownload = true;
                                IsRead = "Yes";
                            }
                            else
                            {
                                permissionModel.IsRead = false; permissionModel.IsDownload = false;
                                IsRead = "No";
                            }
                            @ViewBag.isDownload = IsRead;
                            HttpContext.Session.SetString("isDownload", IsRead);
                            HttpContext.Session.SetString("isView", IsRead);
                        }
                        else
                        {
                            var groupPlist = from tp in _context.EmailConversationParticipantUserGroup
                                             join ugu in _context.UserGroupUser on tp.GroupId equals ugu.UserGroupID
                                             where tp.TopicId == topicId && ugu.UserID == userId
                                             select tp.ID;

                            var gresult = groupPlist.ToList();
                            if (gresult != null)
                            {
                                permissionModel.IsRead = true; permissionModel.IsDownload = true;
                                IsRead = "Yes";
                            }
                            else
                            {
                                permissionModel.IsRead = false; permissionModel.IsDownload = false;
                                IsRead = "No";
                            }
                            @ViewBag.isDownload = IsRead;
                            HttpContext.Session.SetString("isDownload", IsRead);
                            HttpContext.Session.SetString("isView", IsRead);

                        }


                    }
                }
            }
            else if (mode == "ToDo")
            {
                if (documents?.AddedByUserId == userId)
                {
                    permissionModel.IsRead = true; permissionModel.IsDownload = true;
                    IsRead = "Yes";
                }
                else
                {
                    IsRead = "No";
                    permissionModel.IsRead = false; permissionModel.IsDownload = false;
                }

                @ViewBag.isDownload = IsRead;
                HttpContext.Session.SetString("isDownload", IsRead);
                HttpContext.Session.SetString("isView", IsRead);
            }
            else
            {
                permissionModel.IsRead = false; permissionModel.IsDownload = false;
                @ViewBag.isDownload = IsRead;
                HttpContext.Session.SetString("isDownload", IsRead);
                HttpContext.Session.SetString("isView", IsRead);
            }
            return permissionModel;

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
        private byte[] GetOutLookMailDocuments(Stream stream, string? extension)
        {
            List<FileNameModel> files = new List<FileNameModel>();
            var SessionId = Guid.NewGuid();
            var serverPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;
            if (!System.IO.Directory.Exists(serverPaths))
            {
                System.IO.Directory.CreateDirectory(serverPaths);
            }
            string plainTextBytes = string.Empty;
            if (extension == "msg")
            {
                using (var msg = new MsgReader.Outlook.Storage.Message(stream))
                {
                    var attachments = msg.Attachments;
                    if (attachments != null && attachments.Count > 0)
                    {
                        attachments.ForEach(a =>
                        {
                            var attachment = a as MsgReader.Outlook.Storage.Message.Attachment;
                            if (attachment != null && attachment.Hidden == false)
                            {
                                var filePath = serverPaths + @"\" + attachment.FileName;
                                FileNameModel fileNameModel = new FileNameModel();
                                fileNameModel.FilePath = filePath;
                                fileNameModel.FileName = attachment.FileName;
                                files.Add(fileNameModel);
                                using (var streams = new MemoryStream(attachment.Data))
                                {
                                    System.IO.File.WriteAllBytes(filePath, streams.ToArray());
                                }
                            }
                        });
                    }
                }
            }
            if (extension == "eml")
            {
                var eml = MsgReader.Mime.Message.Load(stream);
                if (eml.HtmlBody != null)
                {
                    plainTextBytes = System.Text.Encoding.UTF8.GetString(eml.HtmlBody.Body);
                }
                var attachments = eml.Attachments;
                if (attachments != null && attachments.Count > 0)
                {
                    attachments.ToList().ForEach(attachment =>
                    {
                        if (attachment.ContentId == null && attachment.IsInline == false)
                        {
                            var filePath = serverPaths + @"\" + attachment.FileName;
                            FileNameModel fileNameModel = new FileNameModel();
                            fileNameModel.FilePath = filePath;
                            fileNameModel.FileName = attachment.FileName;
                            files.Add(fileNameModel);
                            using (var streams = new MemoryStream(attachment.Body))
                            {
                                System.IO.File.WriteAllBytes(filePath, streams.ToArray());
                            }
                        }
                    });
                }
            }
            byte[]? stream1 = null;
            if (files.Count > 0)
            {
                var serverZipPaths = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId + @"\" + SessionId + ".zip";
                using (FileStream fs = new FileStream(serverZipPaths, FileMode.Create))
                {
                    using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
                    {
                        files.ForEach(s =>
                        {
                            arch.CreateEntryFromFile(s.FilePath, s.FileName);
                        });
                    }
                }
                if (System.IO.File.Exists(serverZipPaths))
                {
                    stream1 = System.IO.File.ReadAllBytes(serverZipPaths);
                }
                files.ForEach(s =>
                {
                    System.IO.File.Delete(s.FilePath);
                });
                System.IO.File.Delete(serverZipPaths);
                System.IO.Directory.Delete(serverPaths);
            }
            return stream1;
        }


        public static void AddWaterMarks(PdfContentByte dc, string text, BaseFont font, float fontSize, float angle, BaseColor color, Rectangle realPageSize, Rectangle rect = null)
        {
            var gstate = new PdfGState { FillOpacity = 0.35f, StrokeOpacity = 0.3f };
            dc.SaveState();
            dc.SetGState(gstate);
            dc.SetColorFill(color);
            dc.BeginText();
            dc.SetFontAndSize(font, fontSize);
            var ps = rect ?? realPageSize; /*dc.PdfDocument.PageSize is not always correct*/
            var x = (ps.Right + ps.Left) / 2;
            var y = (ps.Bottom + ps.Top) / 2;
            dc.ShowTextAligned(Element.ALIGN_CENTER, text, x, y, angle);
            dc.EndText();
            dc.RestoreState();
        }
    }

}