using AC.SD.Core.Pages.Forum;
using Application.Queries;
using AutoMapper;
using Core.Entities;
using Core.Repositories.Query;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo;
using Infrastructure.Repository.Query;
using MediatR;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Services;
using System.Text;
using static DevExpress.Xpo.Helpers.AssociatedCollectionCriteriaHelper;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using net.tipstrade.FCMNet.Responses;
using System.Configuration;
using DevExpress.XtraRichEdit.Import.Html;
using DevExpress.DataProcessing.InMemoryDataProcessor;
using Core.EntityModels;
using System.Net;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using DevExpress.DocumentServices.ServiceModel.DataContracts;
using AC.SD.Core.Pages.Email;
using Google.Cloud.Firestore;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using System.Dynamic;
using System.Text.Json;
using static iText.Svg.SvgConstants;
using System.Text.Json.Nodes;
using System.Net.Http;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;
using FirebaseNet.Messaging;
using Google.Apis.Auth.OAuth2;
using DevExpress.CodeParser;
using System.Drawing;
using Microsoft.Ajax.Utilities;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IDocumentsQueryRepository _documentsqueryrepository;
        private readonly FirestoreDb _db;
        private readonly HttpClient _httpClient;
        
        public EmailController(IWebHostEnvironment host,IMediator mediator, IApplicationUserQueryRepository applicationUserQueryRepository, IConfiguration configuration, IDocumentsQueryRepository documentsqueryrepository, FirestoreDb db, HttpClient httpClient)
        {
            _hostingEnvironment = host;
            _mediator = mediator;
            _applicationUserQueryRepository = applicationUserQueryRepository;
            _configuration = configuration;
            _documentsqueryrepository = documentsqueryrepository;
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _httpClient = httpClient;
        }

        
        [HttpGet("GetEmailList")]
        public async Task<ActionResult<ResponseModel<List<EmailTopicViewModel>>>> GetList(string mode, long UserId, string SearchTxt,int pageNumber, int pageSize)
        {
            List<Core.Entities.EmailTopics> result = null;
            var response = new ResponseModel<EmailTopicViewModel>();

            if (mode == "To")
            {
                result = await _mediator.Send(new GetEmailTopicTo(UserId, SearchTxt, pageNumber, pageSize));
            }
            else if (mode == "CC")
            {
                result = await _mediator.Send(new GetEmailTopicCC(UserId, SearchTxt));
            }
            else if (mode == "Sent")
            {
                result = await _mediator.Send(new GetSentTopic(UserId, SearchTxt));
            }
            else if (mode == "All")
            {
                result = await _mediator.Send(new GetEmailTopicAll(UserId, SearchTxt, pageNumber, pageSize));
            }
            else
            {
                result = new List<Core.Entities.EmailTopics>();
            }

            var displayResult = result?.Select(topic => new EmailTopicViewModel
            {
                id = topic.ID,
                topicName = topic.TopicName,
                firstName = topic.FirstName,
                lastName = topic.LastName,
                urgent = topic.Urgent,
                isAllowParticipants = topic.IsAllowParticipants,
                dueDate = topic.DueDate,
                onBehalf = topic.OnBehalf,
                onBehalfName = topic.OnBehalfName,
                addedDate = topic.StartDate,
                addedByUserID = topic.AddedByUserID,
                mode = mode,
                userId = UserId,
                addedDateYear = topic.StartDate.Year,
                addedDateDay = topic.StartDate.ToString("dd-MMM"),
                addedTime = topic.StartDate.ToString("hh:mm tt")
            }).ToList();

            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = displayResult;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetEmailSearchList")]
        public async Task<ActionResult<ResponseModel<List<EmailTopicViewModel>>>> GetSearchList(string mode, string SearchTxt, long UserId)
        {
            List<Core.Entities.EmailTopics> result = null;
            var response = new ResponseModel<EmailTopicViewModel>();

            if (mode == "To")
            {
                result = await _mediator.Send(new GetEmailTopicToSearch(SearchTxt, UserId));
            }
            else if (mode == "CC")
            {
                result = await _mediator.Send(new GetEmailTopicCCSearch(SearchTxt, UserId));
            }
            else if (mode == "Sent")
            {
                result = await _mediator.Send(new GetSentTopicSearch(SearchTxt, UserId));
            }
            else if (mode == "All")
            {
                result = await _mediator.Send(new GetEmailTopicAllSearch(SearchTxt, UserId));
            }
            else
            {
                result = new List<Core.Entities.EmailTopics>();
            }

            var displayResult = result?.Select(topic => new EmailTopicViewModel
            {
                id = topic.ID,
                topicName = topic.TopicName,
                firstName = topic.FirstName,
                lastName = topic.LastName,
                urgent = topic.Urgent,
                isAllowParticipants = topic.IsAllowParticipants,
                dueDate = topic.DueDate,
                onBehalf = topic.OnBehalf,
                onBehalfName = topic.OnBehalfName,
                addedDate = topic.StartDate,
                addedByUserID = topic.AddedByUserID,
                mode = mode,
                userId = UserId,
                addedDateYear = topic.StartDate.Year,
                addedDateDay = topic.StartDate.ToString("dd-MMM"),
                addedTime = topic.StartDate.ToString("hh:mm tt")
            }).ToList();

            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = displayResult;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetSubEmailList")]
        public async Task<ActionResult<ResponseModel<List<EmailTopicViewModel>>>> GetSubList(string mode, long Id, long UserId, string SearchTxt)
        {
            List<Core.Entities.EmailTopics> subResult = null;
            var response = new ResponseModel<EmailTopicViewModel>();

            if (mode == "To")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicTo(Id, UserId, SearchTxt));
            }
            else if (mode == "CC")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicCC(Id, UserId, SearchTxt));
            }
            else if (mode == "Sent")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicSent(Id, UserId, SearchTxt));
            }
            else if (mode == "All")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicAll(Id, UserId, SearchTxt));
            }
            else
            {
                subResult = new List<Core.Entities.EmailTopics>();
            }

            var displayResults = subResult?.Select(topic => new EmailTopicViewModel
            {
                id = topic.ID,
                replyId = topic.ReplyId,
                topicName = topic.TopicName,
                firstName = topic.FirstName,
                lastName = topic.LastName,
                urgent = topic.Urgent,
                isAllowParticipants = topic.IsAllowParticipants,
                dueDate = topic.DueDate,
                onBehalf = topic.OnBehalf,
                onBehalfName = topic.OnBehalfName,
                addedDate = topic.StartDate,
                addedByUserID = topic.AddedByUserID,
                mode = mode,
                userId = UserId,
                UserType = topic.UserType,
                addedDateYear = topic.StartDate.Year,
                addedDateDay = topic.StartDate.ToString("dd-MMM"),
                addedTime = topic.StartDate.ToString("hh:mm tt")
            }).ToList();

            try
            {
                response.ResponseCode = ResponseCode.Success;

                // Add AutoMapper configuration here
                //var config = new MapperConfiguration(cfg => {
                //    cfg.CreateMap<EmailTopicViewModel, EmailTopics>();
                //    cfg.CreateMap<EmailTopics, EmailTopicViewModel>();
                //});


                //var mapper = new Mapper(config);
                //response.Results = mapper.Map<List<EmailTopics>>(displayResult);
                response.Results = displayResults;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetDiscussionList")]
        public async Task<ActionResult<ResponseModel<IEnumerable<EmailConversationViewModel>>>> GetEmailDiscussionList(long Id, long UserId)
        {
            var response = new ResponseModel<EmailConversationViewModel>();
            var result = await _mediator.Send(new GetEmailPrintReplyDiscussionList(Id, UserId));

            var displayResult = result?.Select(conversations => new EmailConversationViewModel
            {
                id = conversations.ID,
                replyId = conversations.ReplyId,
                topicId = conversations.TopicID,
                topicName = conversations.Name,
                firstName = conversations.FirstName,
                lastName = conversations.LastName,
                To = conversations.AssignToList,
                CC = conversations.AssignCCList,
                message = conversations.FileData,
                replyCount = conversations.ReplyConversation.Count(),
                ReplyConversation = conversations.ReplyConversation?.Select(rconversation => new Models.ReplyConversationModel
                {
                    id = rconversation.ID,
                    replyId = rconversation.ReplyId,
                    participantId = rconversation.ParticipantId,
                    userName = rconversation.UserName,
                    TopicName = rconversation.Name,
                    firstName = rconversation.FirstName,
                    Message = rconversation.FileData,
                    To = rconversation.AssignToList,
                    CC = rconversation.AssignCCList,
                    userId = rconversation.UserId,
                    isRead = rconversation.IsRead,
                    emailNotificationId = rconversation.EmailNotificationId,
                    urgent = rconversation.Urgent,
                    addedDate = rconversation.AddedDate,
                    sessionId = rconversation.SessionId,
                    isAllowParticipants = rconversation.IsAllowParticipants,
                    addedDateYear = rconversation.AddedDate.Year,
                    addedDateDay = rconversation.AddedDate.ToString("dd-MMM"),
                    addedTime = rconversation.AddedDate.ToString("hh:mm tt"),


                }).ToList(),
                urgent = conversations.Urgent,
                isAllowParticipants = conversations.IsAllowParticipants,
                dueDate = conversations.DueDate,
                onBehalf = conversations.OnBehalf,
                onBehalfName = conversations.OnBehalfName,
                addedDate = conversations.AddedDate,
                addedDateYear = conversations.AddedDate.Year,
                addedDateDay = conversations.AddedDate.ToString("dd-MMM"),
                addedTime = conversations.AddedDate.ToString("hh:mm tt"),
                addedByUserID = conversations.AddedByUserID,
                userId = UserId
            }).ToList();


            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Result = displayResult.FirstOrDefault();
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetOnReplyList")]
        public async Task<IActionResult> GetOnReplyList([FromQuery] long Id, [FromQuery] long UserId)
        {
            var result = await _mediator.Send(new OnReplyConversationTopic(Id, UserId));
            return Ok(result);
        }

        [HttpGet("GetOnDropDownList")]
        public async Task<IActionResult> GetOnDropDownList(string ToIds, string CcIds)
        {
            var result = await _mediator.Send(new OnReplyDropDown(ToIds, CcIds));
            return Ok(result);
        }
        [HttpPost("OnSubmitReply")]
        public async Task<ActionResult<ResponseModel<IEnumerable<ReplyConversation>>>> OnSubmitReply(EmailConversations emailConversations)
        {
            var response = new ResponseModel<ReplyConversation>();

            if (emailConversations.AssigntoIds != null)
            {
                try
                {
                    response.ResponseCode = ResponseCode.Success;


                    List<long> concatList;
                    List<long> NotificationconcatList = new List<long>();
                    string plistUniqueItems = null;

                    if (emailConversations.AssignccIds != null)
                    {
                        concatList = emailConversations.AssigntoIds.Concat(emailConversations.AssignccIds).ToList();
                    }
                    else
                    {
                        concatList = emailConversations.AssigntoIds.ToList();
                    }

                    concatList.Add(emailConversations.ParticipantId);
                    concatList = concatList.Distinct().ToList();
                    string participantidsString = string.Join(",", concatList);

                    string assignccidsString = null;
                    if (emailConversations.AssignccIds != null)
                    {
                        assignccidsString = string.Join(",", emailConversations.AssignccIds);
                    }
                    string assigntoidsString = string.Join(",", emailConversations.AssigntoIds);



                    var plist = await _mediator.Send(new GetByConvasationPList(emailConversations.ReplyId));
                    List<long> plst = plist.Select(x => x.UserID.GetValueOrDefault()).ToList();
                    NotificationconcatList = plst;
                    List<long> uniqueItems = concatList.Except(plst).ToList();
                    if (uniqueItems.Count > 0)
                    {
                        plistUniqueItems = string.Join(",", uniqueItems);
                        NotificationconcatList.AddRange(uniqueItems);
                    }


                    string input_string = emailConversations.Message;
                    byte[] bytes = Encoding.UTF8.GetBytes(input_string);
                    string encodedString = Encoding.UTF8.GetString(bytes);
                    string htmlContent = $"<html><body>{encodedString}</body></html>";
                    byte[] htmlBinaryData = Encoding.UTF8.GetBytes(htmlContent);


                    var createReq = new CreateEmailCoversation
                    {
                        TopicID = emailConversations.TopicID,
                        FileData = htmlBinaryData,
                        Message = emailConversations.Message,
                        AssigntoIdss = assigntoidsString,
                        AssignccIdss = assignccidsString,
                        PlistIdss = participantidsString,
                        AllowPlistids = plistUniqueItems,
                        DueDate = emailConversations.DueDate,
                        IsAllowParticipants = emailConversations.IsAllowParticipants,
                        Urgent = emailConversations.Urgent,
                        ConIds = emailConversations.ReplyId,
                        AllParticipantIds = concatList,
                        ParticipantId = emailConversations.ParticipantId,
                        ReplyId = emailConversations.ReplyId,
                        StatusCodeID = 1,
                        AddedByUserID = emailConversations.ParticipantId,
                        IsMobile = 1,
                        AddedDate = DateTime.Now,
                        Name = emailConversations.Name,
                        SessionId = Guid.NewGuid(),
                        UserType = emailConversations.UserType
                    };

                    var res = await _mediator.Send(createReq);

                    if(emailConversations.FirebaseDocKey != null)
                    {
                        List<string> keyList = emailConversations.FirebaseDocKey.Split(',').Select(s => s.Trim()).ToList();

                        foreach (string key in keyList)
                        {
                            await EmailUploadFile(createReq.SessionId, createReq.AddedByUserID, key);
                        }
                    }

                    var emailconversations = new ReplyConversation
                    {
                        id = (int)res,
                        Message = "Add Successfully"
                    };

                    response.Result = emailconversations;
                    await SendMessage(response.Result.id);
                }
                catch (Exception ex)
                {
                    response.ResponseCode = ResponseCode.Failure;
                    response.ErrorMessages.Add(ex.Message);
                }


                //var result = await _mediator.Send(createReq);
                //return Ok(result);
            }
            else
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add("Your request is invalid. Please check your Assign To");
            }


            return Ok(response);

        }

        [HttpGet("SendMessage")]
        public async Task<string> SendMessage(long id)
        {
            var serverToken = _configuration["FcmNotification:ServerKey"];
            var baseurl = _configuration["DocumentsUrl:BaseUrl"];

            var itm = await _mediator.Send(new GetByIdConversation(id));

            var sid = await _mediator.Send(new GetByIdEmailTopics(itm.TopicID));

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

            var Result = await _mediator.Send(new GetAllConvAssToListQuery(id));

            List<string> tokenStringList = new List<string>();

            var hosturls = baseurl + "ViewEmail/" + sid[0].SessionId;
            foreach (var item in Result)
            {
                var tokens = await _mediator.Send(new GetUserTokenListQuery(item.UserID.Value));
                if (tokens.Count > 0)
                {
                    foreach (var lst in tokens)
                    {
                        //tokenStringList.Add(lst.TokenID.ToString());

                       await PushNotification(lst.TokenID.ToString(), title, bodymsg,lst.DeviceType == "Mobile"? "" :hosturls);
                    }

                }


            }

            return "ok";
        }

        //[HttpGet("PushNotification")]
        //public async Task<string> PushNotification(string token, string title, string message, string hosturl)
        //{
        //    var serverToken = _configuration["FcmNotification:ServerKey"];
        //    var baseurl = _configuration["DocumentsUrl:BaseUrl"];
        //    var pushNotificationRequest = new
        //    {
        //        notification = new
        //        {
        //            title = title,
        //            body = message,
        //            icon = baseurl + "_content/AC.SD.Core/images/SWLogo.png",
        //            click_action = hosturl
        //        },
        //        //data = new Dictionary<string, string>
        //        //{
        //        //    { "url", hosturl}
        //        //},              
        //        registration_ids = new List<string> { token }
        //        //registration_ids = token
        //    };

        //    string url = "https://fcm.googleapis.com/fcm/send";
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("key", "=" + serverToken);

        //        string serializeRequest = JsonConvert.SerializeObject(pushNotificationRequest);
        //        var response = await client.PostAsync(url, new StringContent(serializeRequest, Encoding.UTF8, "application/json"));
        //    }
        //    return "ok";
        //}

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
                    Console.WriteLine(responseContent);

                    return responseContent; // Return response or analyze as needed
                }
                catch (Exception ex)
                {
                    // Log exceptions for further analysis
                    Console.WriteLine($"Error sending notification: {ex.Message}");
                    return $"Error: {ex.Message}";
                }
            }
        }
        [HttpGet("PushNotificationAll")]
        public async Task<string> PushNotificationAll(List<string> tokens, string titles, string message,string housturl)
        {
            
           
            foreach (var lst in tokens)
            {
                //tokenStringList.Add(lst.TokenID.ToString());

                await PushNotification(lst, titles, message, housturl);
            }
            return titles;
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






        [HttpGet("GetDocumentList")]
        public async Task<ActionResult<ResponseModel<List<DocumentsView>>>> GetDocumentList(long id)
        {
            var DocumentViewUrl = _configuration["DocumentsUrl:DocumentViewer"];

            var response = new ResponseModel<DocumentsView>();
            try
            {
                response.ResponseCode = ResponseCode.Success;
                var items = await _mediator.Send(new GetSubEmailTopicDocList(id));


                var displayResult = items?.Select(doc => new DocumentsView
                {
                    DocumentId = doc.DocumentId,
                    SubjectName = doc.SubjectName,
                    FileName = doc.FileName,
                    AddedBy = doc.AddedBy,
                    AddedDate = doc.AddedDate,
                    FilePath = DocumentViewUrl + "?url=" + doc.UniqueSessionId
                }).ToList();
                response.Results = displayResult;
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }

        [HttpPost("EmailUploadFile")]
        public async Task<string> EmailUploadFile(Guid? SessionId, long? UserId,string Id)
        {
            var document = _db.Collection("UploadDocument").Document(Id);
            var snapshot = await document.GetSnapshotAsync();
            var lst = snapshot.Exists ? snapshot.ConvertTo<FirebaseEmailDoc>() : null;
            string firebaseStorageUrl =  null;
            if (lst != null)
            {
                if(lst.Filepath != null)
                {
                     firebaseStorageUrl = lst.Filepath;
                }
                else
                {
                    if(lst.Imagepath != null)
                    {
                        firebaseStorageUrl = lst.Imagepath;
                    }                     
                }

                var serverPathss = _hostingEnvironment.ContentRootPath + @"\AppUpload\Documents\" + SessionId;
                if(firebaseStorageUrl != null)
                {
                    int queryIndex = firebaseStorageUrl.IndexOf('?');
                    string urlWithoutQuery = firebaseStorageUrl.Substring(0, queryIndex);
                    // Find the last occurrence of '.' after removing the query parameters
                    int dotIndex = urlWithoutQuery.LastIndexOf('.');
                    // Extract the extension
                    string extension = urlWithoutQuery.Substring(dotIndex);

                    string fileName = Guid.NewGuid().ToString() + extension;

                    await DownloadFileFromFirebaseStorage(firebaseStorageUrl, serverPathss, fileName);

                    var filePath = Path.Combine(serverPathss, fileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        long fileSize = new FileInfo(filePath).Length;
                        string contentType = GetContentType(filePath);
                        string fileNames = Path.GetFileName(filePath);


                        Documents documents = new Documents();
                        documents.UploadDate = DateTime.Now;
                        documents.AddedByUserId = UserId;
                        documents.AddedDate = DateTime.Now;
                        documents.SessionId = SessionId;
                        documents.IsLatest = true;
                        documents.IsTemp = true;
                        documents.FileName = fileNames;
                        documents.ContentType = contentType;
                        documents.FileSize = fileSize;
                        documents.SourceFrom = "Email";
                        documents.FilePath = filePath.Replace(_hostingEnvironment.ContentRootPath + @"\AppUpload\", "");
                        var response = await _documentsqueryrepository.InsertCreateDocumentBySession(documents);
                        //documentId = response.DocumentId;
                        System.GC.Collect();
                        GC.SuppressFinalize(this);

                        var _documents = _db.Collection("UploadDocument").Document(Id);
                        await _documents.DeleteAsync();


                    }
                    else
                    {

                    }
                }
               
            }
            else
            {

            }
            

            return "ok";
        }
        static async Task DownloadFileFromFirebaseStorage(string url, string destinationFolderPath, string fileName)
        {
            
            
            if (!System.IO.Directory.Exists(destinationFolderPath))
            {
                System.IO.Directory.CreateDirectory(destinationFolderPath);
            }

            string destinationFilePath = Path.Combine(destinationFolderPath, fileName);

            using (WebClient client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), destinationFilePath);
            }
        }
        private string GetContentType(string path)
        {
            string contentType = "application/octet-stream"; // Default content type

            string extension = Path.GetExtension(path).ToLowerInvariant();

            // Content type mappings based on file extension
            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".txt":
                    contentType = "text/plain";
                    break;
                case ".html":
                    contentType = "text/html";
                    break;
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".xml":
                    contentType = "application/xml";
                    break;
                case ".json":
                    contentType = "application/json";
                    break;
                case ".csv":
                    contentType = "text/csv";
                    break;
                case ".mp3":
                case ".wav":
                    contentType = "audio/mpeg";
                    break;
            }

            return contentType;
        }

        [HttpPost("GetDropDownOptionsList")]
        public async Task<ActionResult<ResponseModel<List<DropDownMasterListModel>>>> GetDropDownOptionsList([FromBody] RequestModel request)
        {
           if(request.DynamicsData .ContainsKey("0"))
            {
                request.DynamicsData.Remove("0");
            }
           
        
            var response = new ResponseModel<DropDownMasterListModel>();            
            var results = await _mediator.Send(new GetApplicationMasterParentMobileByList(request.DynamicsData, request.Id));

            var _ActivityStatusItem = results.Where(x => x.Value == "Routine Status").Select(MapToDropDownMasterListModel).ToList();
            var _ActivityResult = results.Where(x => x.Value == "Routine Result").Select(MapToDropDownMasterListModel).ToList();
            var _ActivityMaster = results.Where(x => x.Value == "Routine Info").Select(MapToDropDownMasterListModel).ToList();

            try
            {
                response.ResponseCode = ResponseCode.Success;
                response.Results = new List<DropDownMasterListModel>
                {
                    new DropDownMasterListModel
                    {
                         ActivityStatusItem =  _ActivityStatusItem.Count >0? _ActivityStatusItem : new List<DropDownMasterListModel> { new DropDownMasterListModel() },
                        ActivityResult = _ActivityResult.Count >0? _ActivityResult : new List<DropDownMasterListModel> { new DropDownMasterListModel() },
                        ActivityMaster = _ActivityMaster.Count >0? _ActivityMaster : new List<DropDownMasterListModel> { new DropDownMasterListModel() }
                    }
                };
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }


        private DropDownMasterListModel MapToDropDownMasterListModel(DropDownOptionsListModel item)
        {
            return new DropDownMasterListModel
            {
                Id = item.Id,
                Value = item.Value,
                Text = item.Text
            };
        }

    }

    public class FCMResponse
    {
        public int Success { get; set; }
        public int Failure { get; set; }
        public List<FCMResult> Results { get; set; }
    }

    public class FCMResult
    {
        public string Error { get; set; }
    }

}
