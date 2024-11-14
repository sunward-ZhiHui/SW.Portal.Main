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

        public async Task<IActionResult> Index(string url)
        {
            var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
            var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
            var fileurl = string.Empty; var pathurl = string.Empty;
            HttpContext.Session.Remove("fileName");
            HttpContext.Session.Remove("invalid");
            HttpContext.Session.Remove("isDownload");
            HttpContext.Session.Remove("isView");
            HttpContext.Session.Remove("fileUrl");
            @ViewBag.isDownload = "No";
            var userId = HttpContext.Session.GetString("user_id");
            @ViewBag.isUrl = "isUrl";
            @ViewBag.isFile = "No";
            if (userId != null)
            {
                SpreadsheetDocumentContentFromBytes viewmodel = new SpreadsheetDocumentContentFromBytes();
                if (!string.IsNullOrEmpty(url))
                {
                    var sessionId = new Guid(url);
                    var currentDocuments = _context.Documents.Where(w => w.UniqueSessionId == sessionId).FirstOrDefault();
                    if (currentDocuments != null)
                    {
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
                            }
                            else
                            {
                                fileurl = fileOldUrl + currentDocuments.FilePath;
                                pathurl = _configuration["DocumentsUrl:OldFileLivePath"] + @"\\" + currentDocuments.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
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
                            if (!string.IsNullOrEmpty(fileurl))
                            {
                                string s = viewmodel.Url.Split('.').Last();
                                viewmodel.Extensions = s.ToLower();
                                var uri = new Uri(fileurl);
                                var host = uri.Host;
                                string? contentType = currentDocuments?.ContentType;
                                if (contentType != null)
                                {
                                    var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                                    @ViewBag.FileExtension = Extension;
                                    if (Extension == "xls" || Extension == "xlsx" || Extension == "doc" || Extension == "docx")
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
                                        }
                                        else
                                        {
                                            viewmodel.Id = 0;
                                        }
                                    }
                                    else
                                    {
                                        var webResponse = await webClient.GetAsync(new Uri(fileurl));
                                        var streamData = webResponse.Content.ReadAsStream();
                                        if (Extension == "msg" || Extension == "eml")
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
                                    }
                                    viewmodel.DocumentId = Guid.NewGuid().ToString();
                                    @ViewBag.isFile = "Yes";
                                    viewmodel.ContentType = contentType;
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
                return Redirect("login?url=" + url);
            }
        }
        [HttpPost]
        public IActionResult ExportUrl(string base64, string fileName, DevExpress.AspNetCore.RichEdit.DocumentFormat format, long? id)
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
                if (!string.IsNullOrEmpty(pathurl))
                {
                    byte[] fileContents = System.Convert.FromBase64String(base64);
                    System.IO.File.WriteAllBytesAsync(pathurl, fileContents);
                }
            }
            return Ok();
        }
        public IActionResult RibbonDownloadXlsx(SpreadsheetClientState spreadsheetState)
        {
            var spreadsheet = SpreadsheetRequestProcessor.GetSpreadsheetFromState(spreadsheetState);

            MemoryStream stream = new MemoryStream();
            spreadsheet.SaveCopy(stream, DocumentFormat.Xlsx);
            stream.Position = 0;
            const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, XlsxContentType, HttpContext.Session.GetString("fileName"));
        }
        [HttpPost]
        public void RibbonSaveToFile(SpreadsheetClientState spreadsheetState)
        {
            var spreadsheet = SpreadsheetRequestProcessor.GetSpreadsheetFromState(spreadsheetState);
            spreadsheet.Save();
        }
        public IActionResult RibbonDownloadXls(SpreadsheetClientState spreadsheetState)
        {

            var spreadsheet = SpreadsheetRequestProcessor.GetSpreadsheetFromState(spreadsheetState);

            MemoryStream stream = new MemoryStream();
            spreadsheet.SaveCopy(stream, DocumentFormat.Xls);
            stream.Position = 0;
            const string XlsxContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(stream, XlsxContentType, HttpContext.Session.GetString("fileName"));
        }
        [HttpGet("DownloadFromUrl")]
        public IActionResult DownloadFromUrl(string? url)
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
                    if (currentDocuments != null)
                    {
                        var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                        if (currentDocuments.IsNewPath == true)
                        {
                            fileurl = fileNewUrl + currentDocuments.FilePath;
                        }
                        else
                        {
                            fileurl = fileOldUrl + currentDocuments.FilePath;
                        }
                        if (Extension == "msg")
                        {
                            currentDocuments.ContentType = "application/octet-stream";
                        }
                        var result = DownloadExtention.GetUrlContent(fileurl);
                        if (result != null && result.Result != null)
                        {
                            return File(result.Result, currentDocuments.ContentType, currentDocuments.FileName);
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
        [HttpGet("Maildownloadfromurl")]
        public async Task<IActionResult> Maildownloadfromurl(string? url)
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
                    if (currentDocuments != null)
                    {
                        var Extension = currentDocuments.FileName != null ? currentDocuments.FileName?.Split(".").Last().ToLower() : "";
                        var filnames = currentDocuments.FileName != null ? currentDocuments.FileName?.TrimEnd('.') : "ZipFile";
                        if (currentDocuments.IsNewPath == true)
                        {
                            fileurl = fileNewUrl + currentDocuments.FilePath;
                        }
                        else
                        {
                            fileurl = fileOldUrl + currentDocuments.FilePath;
                        }
                        if (Extension == "msg")
                        {
                            currentDocuments.ContentType = "application/octet-stream";
                        }
                        var webResponse = await webClient.GetAsync(new Uri(fileurl));
                        var streamData = webResponse.Content.ReadAsStream();
                        var streamDatas = GetOutLookMailDocuments(streamData, Extension);
                        if (streamDatas != null)
                        {
                            return File(streamDatas, "application/x-zip-compressed", filnames + ".zip");
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
        [HttpPost]
        public ContentResult DxDocRequest()
        {
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

            var sessionId = currentDocuments?.SessionId;
            var fileProfileId = currentDocuments?.FilterProfileTypeId;
            var documents = _context.Documents.Where(w => w.IsLatest == true && w.SessionId == sessionId && w.SourceFrom == "FileProfile").ToList();
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
                                documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = false, IsUpdateDocument = false, IsRead = true, IsRename = false, IsCopy = false, IsCreateFolder = false, IsEdit = false, IsMove = false, IsShare = false, IsFileDelete = false };
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
                        documentsModels.DocumentPermissionData = new DocumentPermissionModel { IsCreateDocument = false, IsDelete = true, IsUpdateDocument = true, IsRead = true, IsRename = false };
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
                var roleItems = roleItemsList.Where(w => w.FileProfileTypeId == fileProfileId).ToList();
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
                }
                var IsRead = documentsModel.FirstOrDefault()?.DocumentPermissionData.IsRead == true ? "Yes" : "No";
                var isDownload = documentsModel.FirstOrDefault()?.DocumentPermissionData.IsEdit == true ? "Yes" : "No";
                if (userId == documentsModel.FirstOrDefault()?.UploadedByUserId)
                {
                    IsRead = "Yes"; isDownload = "Yes";
                }
                @ViewBag.isDownload = isDownload;
                HttpContext.Session.SetString("isDownload", isDownload);
                HttpContext.Session.SetString("isView", IsRead);

            }
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
                if (res != null && res.Count()>0)
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
    }

}