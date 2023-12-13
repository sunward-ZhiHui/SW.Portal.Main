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

namespace DocumentViewer.Controllers
{
    public class HomeController : Controller
    {

        private static HttpClient webClient = new HttpClient();
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index(string url)
        {
            var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
            var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
            var fileurl = string.Empty;
            HttpContext.Session.Remove("fileName");
            HttpContext.Session.Remove("invalid");
            HttpContext.Session.Remove("isDownload");
            HttpContext.Session.Remove("isView");
            HttpContext.Session.Remove("fileUrl");
            @ViewBag.isDownload = "No";
            var userId = HttpContext.Session.GetString("user_id");
            @ViewBag.isUrl = "isUrl";
            if (userId != null)
            {
                SpreadsheetDocumentContentFromBytes viewmodel = new SpreadsheetDocumentContentFromBytes();
                if (!string.IsNullOrEmpty(url))
                {
                    var sessionId = new Guid(url);
                    var currentDocuments = _context.Documents.Where(w => w.UniqueSessionId == sessionId).FirstOrDefault();
                    if (currentDocuments != null)
                    {
                        HttpContext.Session.SetString("fileName", currentDocuments.FileName);
                        long userIds = Int64.Parse(HttpContext.Session.GetString("user_id"));
                        if (!string.IsNullOrEmpty(currentDocuments.FilePath))
                        {
                            if (currentDocuments.SourceFrom == "FileProfile")
                            {
                                var query = from oal in _context.OpenAccessUserLink
                                            join oau in _context.OpenAccessUser on oal.OpenAccessUserId equals oau.OpenAccessUserId
                                            where oal.UserId == userIds && oau.AccessType == "DMSAccess" && oal.IsDmsAccess==true
                                            select oal;
                                if (query != null)
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
                                var per=GetEmailFilePermissionAsync(currentDocuments);
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
                                fileurl = fileNewUrl + currentDocuments.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
                            }
                            else
                            {
                                fileurl = fileOldUrl + currentDocuments.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
                            }
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
                                string? contentType = currentDocuments?.ContentType;
                                if (contentType != null)
                                {
                                    /* using (var _httpClient = new HttpClient())
                                     {
                                         using (var response = await _httpClient.GetAsync(new Uri(fileurl)))
                                         {
                                             response.EnsureSuccessStatusCode();
                                             Stream byteArrayAccessor() => response.Content.ReadAsStream();
                                             //var stream = await response.Content.ReadAsStreamAsync();
                                             viewmodel.DocumentId = Guid.NewGuid().ToString();
                                             viewmodel.ContentAccessorByBytes = byteArrayAccessor;
                                             viewmodel.Type = contentType.Split("/")[0].ToLower();
                                             viewmodel.ContentType = contentType;
                                             return View(viewmodel);
                                         }
                                     }*/
                                    /*using (var webClient = new HttpClient())
                                    {*/
                                    var webResponse = await webClient.GetAsync(new Uri(fileurl));
                                    Stream byteArrayAccessor() => webResponse.Content.ReadAsStream();
                                    viewmodel.DocumentId = Guid.NewGuid().ToString();
                                    viewmodel.ContentAccessorByBytes = byteArrayAccessor;
                                    viewmodel.Type = contentType.Split("/")[0].ToLower();
                                    viewmodel.ContentType = contentType;
                                    System.GC.Collect();
                                    GC.SuppressFinalize(this);
                                    return View(viewmodel);
                                    //}
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
                        if (currentDocuments.IsNewPath == true)
                        {
                            fileurl = fileNewUrl + currentDocuments.FilePath;
                        }
                        else
                        {
                            fileurl = fileOldUrl + currentDocuments.FilePath;
                        }
                        var result = DownloadExtention.GetUrlContent(fileurl);
                        if (result != null)
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

                if (query != null)
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
    }

}