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

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        private readonly IMediator _mediator;

        public EmailController(IMediator mediator, IApplicationUserQueryRepository applicationUserQueryRepository)
        {
            _mediator = mediator;
            _applicationUserQueryRepository = applicationUserQueryRepository;
        }


        [HttpGet("GetEmailList")]
        public async Task<ActionResult<ResponseModel<IEnumerable<EmailTopics>>>> GetList(string mode, long UserId)
        {
            List<Core.Entities.EmailTopics> result = null;
            var response = new ResponseModel<EmailTopics>();

            if (mode == "To")
            {
                result = await _mediator.Send(new GetEmailTopicTo(UserId));
            }
            else if (mode == "CC")
            {
                result = await _mediator.Send(new GetEmailTopicCC(UserId));
            }
            else if (mode == "Sent")
            {
                result = await _mediator.Send(new GetSentTopic(UserId));
            }
            else if (mode == "All")
            {
                result = await _mediator.Send(new GetEmailTopicAll(UserId));
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
                addedByUserID =  topic.AddedByUserID,
                mode = mode,
                userId = UserId
            }).ToList();

            try
            {
                response.ResponseCode = ResponseCode.Success;

                // Add AutoMapper configuration here
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<EmailTopicViewModel, EmailTopics>();
                    cfg.CreateMap<EmailTopics, EmailTopicViewModel>();
                });


                var mapper = new Mapper(config);               
                response.Results = mapper.Map<List<EmailTopics>>(displayResult);
            }
            catch (Exception ex)
            {
                response.ResponseCode = ResponseCode.Failure;
                response.ErrorMessages.Add(ex.Message);
            }

            return Ok(response);
        }
        [HttpGet("GetSubEmailList")]
        public async Task<ActionResult<ResponseModel<IEnumerable<EmailTopics>>>> GetSubList(string mode, long Id, long UserId)
        {
            List<Core.Entities.EmailTopics> subResult = null;
            var response = new ResponseModel<EmailTopics>();

            if (mode == "To")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicTo(Id, UserId));
            }
            else if (mode == "CC")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicCC(Id, UserId));
            }
            else if (mode == "Sent")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicSent(Id, UserId));
            }
            else if (mode == "All")
            {
                subResult = await _mediator.Send(new GetSubEmailTopicAll(Id, UserId));
            }
            else
            {
                subResult = new List<Core.Entities.EmailTopics>();
            }

            var displayResult = subResult?.Select(topic => new EmailTopicViewModel
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
                userId = UserId
            }).ToList();

            try
            {
                response.ResponseCode = ResponseCode.Success;

                // Add AutoMapper configuration here
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<EmailTopicViewModel, EmailTopics>();
                    cfg.CreateMap<EmailTopics, EmailTopicViewModel>();
                });


                var mapper = new Mapper(config);
                response.Results = mapper.Map<List<EmailTopics>>(displayResult);
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
            var result = await _mediator.Send(new GetEmailReplyDiscussionList(Id, UserId));

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
                ReplyConversation = conversations.ReplyConversation?.Select(rconversation => new ReplyConversationModel
                {
                    id= rconversation.ID,
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
                    

                }).ToList(),               
                urgent = conversations.Urgent,
                isAllowParticipants = conversations.IsAllowParticipants,
                dueDate = conversations.DueDate,
                onBehalf = conversations.OnBehalf,
                onBehalfName = conversations.OnBehalfName,
                addedDate = conversations.AddedDate,
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
        public async Task<IActionResult> GetOnReplyList(long Id, long UserId)
        {
            var result = await _mediator.Send(new OnReplyConversation(Id, UserId));
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
                        SessionId = Guid.NewGuid()
                    };

                    var res = await _mediator.Send(createReq);

                    var emailconversations = new ReplyConversation
                    {
                        id = (int)res,
                        Message = "Add Successfully"
                    };

                    response.Result = emailconversations;
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
        public async Task<string> SendMessage()
        {
            //var result = await _fcm.SendMessageAsync("/topics/news", "My Message Title", "Message Data", "");

            //await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            //var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            var token = new List<string> 
            {
                "c-1W1o1jvnE:APA91bFnLX74Xdwn1ej0ptWDHNxMoaVCBV-yJ_14T3OZnaNwudTJcDHdkNFyiQ6fOLkktnUqOrv4nTUBvFXyVv1zkF_spGswkVyXmkMXfEWVtfdtbgc4ox02-P6MsQ5cphnzX0UFlaQQ"
                
            };

            var androidNotificationObject = new Dictionary<string, string>();
            var pushNotificationRequest = new PostItem
            {
                notification = new NotificationMessageBody
                {
                    title = "Title",
                    body = "Welcome to all"
                },
                data = androidNotificationObject,
                //registration_ids = new List<string> { token }
                registration_ids = token 
            };

            string url = "https://fcm.googleapis.com/fcm/send";
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("key", "=" + "AAAAeo31_Is:APA91bFPh3rj_ZrmfurBTfz_Ahw_Ojo9rA4oNIFoaNThAHUhwtq515F19qD9ICngHp5qs1IBQ1ZePalvD8YOzCKF-va991eN02_TEZtgAE4AWM5hku9rDdQoEZvT47l3mE67LcGpKMuz");

                string serializeRequest = JsonConvert.SerializeObject(pushNotificationRequest);
                var response = await client.PostAsync(url, new StringContent(serializeRequest, Encoding.UTF8, "application/json"));
                //if (response.StatusCode == System.Net.HttpStatusCode.OK)
                //{
                //    //await App.Current.MainPage.DisplayAlert("Notification sent", "notification sent", "OK");
                //}
            }
            return "ok";
        }
    }
}
