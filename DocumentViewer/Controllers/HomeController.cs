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

namespace DocumentViewer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, AppDbContext context, IConfiguration configuration)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Index(string url)
        {
            var fileOldUrl = _configuration["DocumentsUrl:FileOldUrl"];
            var fileNewUrl = _configuration["DocumentsUrl:FileNewUrl"];
            var fileurl = string.Empty;
            HttpContext.Session.Remove("isDownload");
            HttpContext.Session.Remove("isView");
            HttpContext.Session.Remove("fileUrl");
            var userId = HttpContext.Session.GetString("user_id");
            if (userId != null)
            {
                if (!string.IsNullOrEmpty(url))
                {
                    var sessionId = new Guid(url);
                    var currentDocuments = _context.Documents.Where(w => w.UniqueSessionId == sessionId).FirstOrDefault();
                    if (currentDocuments != null)
                    {
                        if (!string.IsNullOrEmpty(currentDocuments.FilePath))
                        {
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
                            if (currentDocuments.FilterProfileTypeId > 0)
                            {
                                GetAllSelectedFilePermissionAsync(currentDocuments);
                            }
                        }
                    }
                    else
                    {
                        var currentDocumentss = _context.Documents.Where(w => w.SessionId == sessionId).FirstOrDefault();
                        if (currentDocumentss != null && !string.IsNullOrEmpty(currentDocumentss.FilePath))
                        {
                            if (currentDocumentss.IsNewPath == true)
                            {
                                fileurl = fileNewUrl + currentDocumentss.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
                            }
                            else
                            {
                                fileurl = fileOldUrl + currentDocumentss.FilePath;
                                HttpContext.Session.SetString("fileUrl", fileurl);
                            }
                            if (currentDocumentss.FilterProfileTypeId > 0)
                            {
                                GetAllSelectedFilePermissionAsync(currentDocumentss);
                            }
                        }
                    }
                }
                SpreadsheetDocumentContentFromBytes viewmodel = new SpreadsheetDocumentContentFromBytes();
                try
                {
                    viewmodel.Extensions = "";
                    viewmodel.Url = string.IsNullOrEmpty(fileurl) ? "" : fileurl;
                    viewmodel.Id = 1;
                    viewmodel.DocumentId = "1";
                    if (!string.IsNullOrEmpty(fileurl))
                    {
                        string s = viewmodel.Url.Split('.').Last();
                        viewmodel.Extensions = s.ToLower();
                        var uri = new Uri(fileurl);
                        var host = uri.Host;
                        // if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        // {
                        string contentType = "";
                        var request = HttpWebRequest.Create(fileurl) as HttpWebRequest;
                        if (request != null)
                        {
                            var response = request.GetResponse() as HttpWebResponse;
                            if (response != null)
                                contentType = response.ContentType;
                        }
                        if (contentType != null)
                        {
                            var webClient = new WebClient();
                            {
                                byte[] byteArrayAccessor() => webClient.DownloadData(new Uri(fileurl));
                                viewmodel.DocumentId = Guid.NewGuid().ToString();
                                viewmodel.ContentAccessorByBytes = byteArrayAccessor;
                                viewmodel.Type = contentType.Split("/")[0].ToLower();
                                viewmodel.ContentType = contentType;
                                return View(viewmodel);
                            }
                            //}
                            /* else
                             {
                                 viewmodel.DocumentId = "0";
                                 viewmodel.ContentType = contentType;
                                 return View(viewmodel);
                             }*/
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
                    //throw new Exception(ex.Message);
                }

            }
            else
            {
                return Redirect("login?url=" + url);
            }
        }
        [HttpGet("DownloadFromUrl")]
        public IActionResult DownloadFromUrl()
        {
            try
            {
                string url = HttpContext.Session.GetString("fileUrl");
                var result = DownloadExtention.GetUrlContent(url);
                var request = HttpWebRequest.Create(url) as HttpWebRequest;
                string contentType = "";
                string filename = "";
                Uri uri = new Uri(url);
                filename = uri.LocalPath.Split("/").Last();
                if (request != null)
                {
                    var response = request.GetResponse() as HttpWebResponse;
                    if (response != null)
                        contentType = response.ContentType;
                }
                if (result != null)
                {
                    return File(result.Result, contentType, filename);
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
        public void GetAllSelectedFilePermissionAsync(Documents currentDocuments)
        {
            List<DocumentsModel> documentsModel = new List<DocumentsModel>();
            long userId = Int64.Parse(HttpContext.Session.GetString("user_id"));

            var sessionId = currentDocuments?.SessionId;
            var fileProfileId = currentDocuments?.FilterProfileTypeId;
            var documents = _context.Documents.Where(w => w.IsLatest == true && w.SessionId == sessionId).ToList();
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
                if (userId == documentsModel.FirstOrDefault()?.UploadedByUserId)
                {
                    IsRead = "Yes";
                }
                HttpContext.Session.SetString("isDownload", IsRead);
                HttpContext.Session.SetString("isView", IsRead);
            }
        }
        public List<DocumentPermissionModel> GetDocumentPermissionByRoll()
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
    }

}