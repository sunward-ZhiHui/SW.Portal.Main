using Azure.Core;
using DevExpress.AspNetCore.Spreadsheet;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using DevExpress.Export.Xl;
using DevExpress.Office.Drawing;
using DevExpress.Spreadsheet;
using DevExpress.Web.Office;
using DevExpress.Xpo;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Utils;
using DocumentApi.Models;
using DocumentViewer.EntityModels;
using DocumentViewer.Helpers;
using DocumentViewer.Models;
using Google.Apis.Auth.OAuth2;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.qrcode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using MsgReader.Outlook;
using Newtonsoft.Json;
using System;
using System.Buffers.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using static DevExpress.Utils.HashCodeHelper.Primitives;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using static DevExpress.XtraPrinting.Native.ExportOptionsPropertiesNames;

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
        [HttpPost("NotifyEmailDocument")]
        public async Task<bool> NotifyEmailDocumentAsync(long? documentId, string? isVersion)
        {
            if (documentId > 0)
            {
                var currentDocuments = _context.Documents.Where(w => w.DocumentId == documentId).FirstOrDefault();
                if (currentDocuments != null)
                {
                    if (currentDocuments.SourceFrom == "Email")
                    {
                        var versiontracking = _context.DocumentsVersionTrace.Where(x => x.DocumentId == documentId).OrderByDescending(x => x.DocumentsVersionTraceId).FirstOrDefault();

                        var userId = (long?)Convert.ToDouble(HttpContext.Session.GetString("user_id"));
                        var latestDocument = _context.Documents.FirstOrDefault(c => c.SessionId == currentDocuments.SessionId && c.IsLatest == true);
                        var emailconversation = _context.EmailConversations.Where(p => p.SessionId == currentDocuments.SessionId).FirstOrDefault();
                        if (emailconversation != null)
                        {

                            var emplist = _context.Employee.Where(p => p.UserId == userId).FirstOrDefault();

                            var lastRecord = _context.EmailConversations
                                .Where(e => e.ReplyId == emailconversation.ReplyId)
                                .OrderByDescending(e => e.ID)
                                .FirstOrDefault();

                            var lastRecordId = lastRecord?.ID;
                            var createReq = lastRecord;

                            var assignTo = _context.EmailConversationAssignTo.Where(x => x.ConversationId == lastRecordId).ToList();
                            var assignCC = _context.EmailConversationAssignCC.Where(x => x.ConversationId == lastRecordId).ToList();


                            //string versionNo = versiontracking?.VersionNo?.ToString() ?? "Not yet versioned";
                            //string description = versiontracking?.Description ?? "No description provided for this version";


                            string versionNo;
                            string description;
                            string userReplyContent;

                            if (string.Equals(isVersion, "Yes", StringComparison.OrdinalIgnoreCase))
                            {
                                versionNo = versiontracking?.VersionNo?.ToString() ?? "Not yet versioned";
                                description = versiontracking?.Description ?? "No description provided.";

                                 userReplyContent = $"Version {versionNo}: {description}";
                            }
                            else
                            {                                

                                userReplyContent = "The document has been updated without a change in the version number. Please find the revised document below.";
                            }




                            //string userReplyContent = $"Version {versionNo}: {description}";
                            string displayName = !string.IsNullOrEmpty(emplist.NickName) ? $"{emplist.FirstName} - {emplist.NickName}" : $"{emplist.FirstName} {emplist.LastName}";

                            string documentName = latestDocument?.FileName ?? "Unknown Document";
                            //string documentUrl = $"/Documents/View/{latestDocument?.UniqueSessionId}"; // adjust to your routing
                            string documentUrl = $"https://portal.sunwardpharma.com/FileViewer/login?url={latestDocument?.UniqueSessionId}";

                            string replyText =
                            $"<strong>Auto Reply Notification</strong><br/><br/>" +
                            //$"User <strong>{applicationUser.FirstName} {applicationUser.LastName}</strong> " +
                            $"User <strong>{displayName}</strong> " +
                            $"has updated the document <strong>{documentName}</strong>.<br/><br/>" +
                            $"<strong>Message:</strong><br/>" +
                            $"&nbsp;&nbsp;&nbsp;&nbsp;{userReplyContent}<br/><br/>" +
                            $"<a href='{documentUrl}' target='_blank' " +
                            $"style='display:inline-block;padding:8px 14px;background-color:#007bff;color:#fff;" +
                            $"text-decoration:none;border-radius:5px;'>View Document</a><br/><br/>" +
                            $"<em>This is an automated reply generated by the system.</em>";

                            //string replyText =
                            //  $"<strong>Auto Reply Notification</strong><br/><br/>" +
                            //  $"User <strong>{displayName}</strong> " +
                            //  $"has updated the document <strong>{documentName}</strong>.<br/><br/>" +
                            //  $"<strong>Message:</strong><br/>" +
                            //  $"&nbsp;&nbsp;&nbsp;&nbsp;{userReplyContent}<br/><br/>" +
                            //  $"<button onclick=\"viewDocumentJs('{latestDocument?.UniqueSessionId}')\" " +
                            //  $"style='padding:8px 14px;background-color:#007bff;color:#fff;border:none;border-radius:5px;cursor:pointer;'>View Document</button>" +
                            //  $"<em>This is an automated reply generated by the system.</em>";


                            // Convert to byte[]
                            byte[] replyBytes = System.Text.Encoding.UTF8.GetBytes(replyText);

                            string textContent = "Auto Reply Notification.";

                            string descriptionContent = textContent.Length > 50 ? textContent.Substring(0, 50) : textContent;

                            //byte[] fileBytes = System.Text.Encoding.UTF8.GetBytes(textContent);

                            var newConversation = new EmailConversation
                            {
                                TopicID = createReq.TopicID,
                                FileData = replyBytes,
                                //Message = createReq.Message,      
                                Description = descriptionContent,
                                DueDate = createReq.DueDate,
                                IsAllowParticipants = createReq.IsAllowParticipants,
                                Urgent = createReq.Urgent,
                                ParticipantId = userId,
                                ReplyId = createReq.ReplyId,
                                StatusCodeID = 1,
                                AddedByUserID = (int?)userId,
                                IsMobile = createReq.IsMobile,
                                AddedDate = DateTime.Now,
                                Name = createReq.Name,
                                SessionId = Guid.NewGuid(),
                                UserType = createReq.UserType,
                                IsDueDate = createReq.IsDueDate,
                                TransferID = createReq.TransferID,
                            };

                            _context.EmailConversations.Add(newConversation);
                            await _context.SaveChangesAsync();

                            var newConversationId = newConversation.ID;

                            if (lastRecord != null && lastRecord.ParticipantId.HasValue && userId.HasValue && lastRecord.ParticipantId.Value != userId.Value)
                            {
                                long currentUserId = userId.Value;
                                long newParticipantId = lastRecord.ParticipantId.Value;

                                // 1. Remove currentUserId from assignTo list
                                assignTo = assignTo.Where(x => x.UserId != currentUserId).ToList();

                                // 2. Remove currentUserId from assignCC list
                                assignCC = assignCC.Where(x => x.UserId != currentUserId).ToList();

                                // 3. Always add lastRecord.ParticipantId into assignTo list
                                if (!assignTo.Any(x => x.UserId == newParticipantId))
                                {
                                    assignTo.Add(new EmailConversationAssignTo
                                    {
                                        ConversationId = newConversationId,   // use new conversation ID
                                        TopicId = lastRecord.TopicID,
                                        UserId = newParticipantId,
                                        StatusCodeID = 1,
                                        AddedByUserID = (int?)currentUserId,
                                        AddedDate = DateTime.Now,
                                        SessionId = Guid.NewGuid()
                                    });
                                }


                                var allAssignments = assignTo.Select(a => new { a.TopicId, a.UserId }).Concat(assignCC.Select(c => new { c.TopicId, c.UserId })).ToList();

                                allAssignments.Add(new { TopicId = lastRecord.TopicID, UserId = userId.Value });
                                allAssignments = allAssignments.GroupBy(x => x.UserId).Select(g => g.First()).ToList();


                                foreach (var assign in assignTo)
                                {
                                    _context.EmailConversationAssignTo.Add(new EmailConversationAssignTo
                                    {
                                        ConversationId = newConversationId,
                                        TopicId = assign.TopicId,
                                        UserId = assign.UserId,
                                        StatusCodeID = 1,
                                        AddedByUserID = (int?)userId,
                                        AddedDate = DateTime.Now,
                                        SessionId = Guid.NewGuid()
                                    });
                                }




                                foreach (var assign in assignCC)
                                {
                                    _context.EmailConversationAssignCC.Add(new EmailConversationAssignCC
                                    {
                                        ConversationId = newConversationId,
                                        TopicId = assign.TopicId,
                                        UserId = assign.UserId,
                                        StatusCodeID = 1,
                                        AddedByUserID = (int?)userId,
                                        AddedDate = DateTime.Now,
                                        SessionId = Guid.NewGuid()
                                    });
                                }



                                // Insert into EmailNotifications
                                foreach (var item in allAssignments)
                                {
                                    _context.EmailNotifications.Add(new EmailNotifications
                                    {
                                        ConversationId = newConversationId,
                                        TopicId = item.TopicId,
                                        UserId = item.UserId,
                                        IsRead = item.UserId == userId ? true : false,
                                        AddedDate = DateTime.Now,
                                        AddedByUserID = userId
                                    });
                                }

                                await _context.SaveChangesAsync();
                            }
                            else
                            {
                                var allAssignments = assignTo.Select(a => new { a.TopicId, a.UserId }).Concat(assignCC.Select(c => new { c.TopicId, c.UserId })).ToList();

                                allAssignments.Add(new { TopicId = lastRecord.TopicID, UserId = userId.Value });
                                allAssignments = allAssignments.GroupBy(x => x.UserId).Select(g => g.First()).ToList();

                                // Original logic when ParticipantId == userId
                                foreach (var assign in assignTo)
                                {
                                    _context.EmailConversationAssignTo.Add(new EmailConversationAssignTo
                                    {
                                        ConversationId = newConversationId,
                                        TopicId = assign.TopicId,
                                        UserId = assign.UserId,
                                        StatusCodeID = 1,
                                        AddedByUserID = (int?)userId,
                                        AddedDate = DateTime.Now,
                                        SessionId = Guid.NewGuid()
                                    });
                                }

                                foreach (var assign in assignCC)
                                {
                                    _context.EmailConversationAssignCC.Add(new EmailConversationAssignCC
                                    {
                                        ConversationId = newConversationId,
                                        TopicId = assign.TopicId,
                                        UserId = assign.UserId,
                                        StatusCodeID = 1,
                                        AddedByUserID = (int?)userId,
                                        AddedDate = DateTime.Now,
                                        SessionId = Guid.NewGuid()
                                    });
                                }






                                // Insert into EmailNotifications
                                foreach (var item in allAssignments)
                                {
                                    _context.EmailNotifications.Add(new EmailNotifications
                                    {
                                        ConversationId = newConversationId,
                                        TopicId = item.TopicId,
                                        UserId = item.UserId,
                                        IsRead = item.UserId == userId ? true : false,
                                        AddedDate = DateTime.Now,
                                        AddedByUserID = userId
                                    });
                                }

                                await _context.SaveChangesAsync();
                            }


                            await SendMessage(newConversationId);


                            //foreach (var assign in assignTo)
                            //{
                            //    _context.EmailConversationAssignTo.Add(new EmailConversationAssignTo
                            //    {
                            //        ConversationId = newConversationId,
                            //        TopicId = assign.TopicId,
                            //        UserId = assign.UserId,
                            //        StatusCodeID = 1,
                            //        AddedByUserID = (int?)userId,
                            //        AddedDate = DateTime.Now,
                            //        SessionId = Guid.NewGuid()
                            //    });
                            //}

                            //foreach (var assign in assignCC)
                            //{
                            //    _context.EmailConversationAssignCC.Add(new EmailConversationAssignCC
                            //    {
                            //        ConversationId = newConversationId,
                            //        TopicId = assign.TopicId,
                            //        UserId = assign.UserId,
                            //        StatusCodeID = 1,
                            //        AddedByUserID = (int?)userId,
                            //        AddedDate = DateTime.Now,
                            //        SessionId = Guid.NewGuid()
                            //    });
                            //}

                            //var rows = await _context.SaveChangesAsync();
                            //Console.WriteLine($"Rows written: {rows}");


                        }


                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
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
                await saveUpdateUserData(isVersion);
            }
            return Ok();
        }
        public void VersionDataData(Documents documents, string? newfilePath, long length)
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
        public async Task<string> RibbonSaveToFile(SpreadsheetClientState spreadsheetState, long? id, string? isVersion)
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
                await saveUpdateUserData(isVersion);                
            }

            return isVersion ?? "No";
        }
        public async Task<string> saveUpdateUserData(string? isVersion)
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
           
            await NotifyEmailDocumentAsync(documentId, isVersion);
            return isVersion;
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
                        var topicId = emailConversationlst.TopicID;
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
                            if (attachment != null && attachment.ContentId != null && attachment.IsInline == true)
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

        //public async Task<EmailConversation?> GetEmailByReplyId(long rid)
        //{
        //    return await _context.EmailConversations.FirstOrDefaultAsync(e => e.ID == rid);
        //}

        public async Task<List<Employee>> GetConversationEmployees(long convId)
        {
            var toEmployees = from ec in _context.EmailConversationAssignTo
                              join e in _context.Employee on ec.UserId equals e.UserId
                              where ec.ConversationId == convId
                              select e;

            var ccEmployees = from ec in _context.EmailConversationAssignCC
                              join e in _context.Employee on ec.UserId equals e.UserId
                              where ec.ConversationId == convId
                              select e;

            var result = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
                .ToListAsync(toEmployees.Union(ccEmployees));

            return result;
        }
        public async Task<(EmailConversation? Item, Guid? ReplySessionId)> GetConversationWithReplySessionId(long id)
        {
            var result = (from e in _context.EmailConversations
                          join r in _context.EmailConversations on e.ReplyId equals r.ID into replies
                          from reply in replies.DefaultIfEmpty()
                          where e.ID == id
                          select new
                          {
                              Item = e,
                              ReplySessionId = reply.SessionId
                          })
               .FirstOrDefault();


            return (result?.Item, result?.ReplySessionId);
        }


        [HttpGet("SendMessage")]
        public async Task<string> SendMessage(long id)
        {
            var serverToken = _configuration["FcmNotification:ServerKey"];
            var baseurl = _configuration["DocumentsUrl:BaseUrl"];



            var (itm, sessionId) = await GetConversationWithReplySessionId(id);



            //var itm = _context.EmailConversations.FirstOrDefault(w => w.ID == id);
            //var sid = _context.EmailConversations.FirstOrDefault(c => c.ID == itm.ReplyId);

            //var itm = _context.EmailConversations.Where(w => w.ID == id).FirstOrDefault();
                  

            string title = itm.Name;

            byte[] htmlBinaryData = itm.FileData; // Your HTML binary data
            string htmlContent = System.Text.Encoding.UTF8.GetString(htmlBinaryData);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            // string extractedText = doc.DocumentNode.InnerText.Trim();

            string extractedText = HtmlEntity.DeEntitize(doc.DocumentNode.InnerText.Trim());

            // Remove unwanted line breaks and whitespace
            extractedText = Regex.Replace(extractedText, @"\s+", " ").Trim();

            string bodymsg = extractedText.Substring(0, Math.Min(20, extractedText.Length));

            var Result = await GetConversationEmployees(id);

            List<string> tokenStringList = new List<string>();

            //var hosturls = baseurl + "ViewEmail/" + sid[0].SessionId;
            var hosturls = baseurl + "ViewEmail/" + sessionId;

            foreach (var item in Result)
            {               

                var tokens = _context.UserNotifications.Where(w => w.UserId == item.UserId && w.DeviceType != "IPIR").ToList();

                //var tokens = await _mediator.Send(new GetUserTokenListQuery(item.UserID.Value));

                if (tokens.Count > 0)
                {
                    foreach (var lst in tokens)
                    {
                        await PushNotification(lst.TokenID.ToString(), title, bodymsg, lst.DeviceType == "Mobile" ? "" : hosturls);
                    }
                }
            }

            return "ok";
        }
        [HttpGet("PushNotification")]
        public async Task<string> PushNotification(string tokens, string titles, string message, string hosturl)
        {
            var baseurl = _configuration["DocumentsUrl:BaseUrl"];
            var projectId = _configuration["FcmNotification:ProjectId"];
            var oauthToken = await GetAccessTokenAsync(_hostingEnvironment);
            var iconUrl = baseurl + "_content/AC.SD.Core/images/SWLogo.png";

            var pushNotificationRequest = new
            {
                message = new
                {
                    token = tokens,
                    notification = new
                    {
                        title = titles,
                        body = message
                    },
                    webpush = new
                    {
                        fcm_options = new
                        {
                            link = hosturl
                        }
                    }
                }
            };

            string url = $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", oauthToken);

                try
                {
                    string serializeRequest = JsonConvert.SerializeObject(pushNotificationRequest);
                    var response = await client.PostAsync(url, new StringContent(serializeRequest, Encoding.UTF8, "application/json"));
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Log response content for debugging
                    //Console.WriteLine(responseContent);

                    // Handle UNREGISTERED token
                    if (!response.IsSuccessStatusCode && responseContent.Contains("UNREGISTERED"))
                    {
                        // Optionally log and remove token
                        Console.WriteLine($"Invalid FCM Token: {tokens}");
                        return "UNREGISTERED";
                    }

                    return responseContent; // Return response or analyze as needed
                }
                catch (Exception ex)
                {
                    // Log exceptions for further analysis
                    //Console.WriteLine($"Error sending notification: {ex.Message}");
                    return $"Error: {ex.Message}";
                }
            }
        }
        private async Task<string> GetAccessTokenAsync(IWebHostEnvironment env)
        {
            string relativePath = _configuration["FcmNotification:FilePath"];

            string path = Path.Combine(env.ContentRootPath, relativePath);

            GoogleCredential credential = await GoogleCredential.FromFileAsync(path, CancellationToken.None);

            credential = credential.CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

            var token = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
            return token;
        }


    }

}