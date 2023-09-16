using AC.SD.Core.Pages.Forum;
using Application.Queries;
using Core.Entities;
using Core.Repositories.Query;
using DevExpress.ClipboardSource.SpreadsheetML;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpo;
using Infrastructure.Repository.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.WsTrust;
using Microsoft.VisualBasic;
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
        public async Task<IActionResult> GetList(string mode, long UserId)
        {
            List<Core.Entities.EmailTopics> result = null;

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
            
            var displayResult = result?.Select(topic => new
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

            return Ok(displayResult);
        }
        [HttpGet("GetSubEmailList")]
        public async Task<IActionResult> GetSubList(string mode, long Id, long UserId)
        {
            List<Core.Entities.EmailTopics> subResult = null;

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

            var displayResult = subResult?.Select(topic => new
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

            return Ok(displayResult);
        }
        [HttpGet("GetDiscussionList")]
        public async Task<IActionResult> GetEmailDiscussionList(long Id, long UserId)
        {
            var result = await _mediator.Send(new GetEmailReplyDiscussionList(Id, UserId));

            var displayResult = result?.Select(conversations => new
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
                ReplyConversation = conversations.ReplyConversation?.Select(rconversation => new
                {
                    id= rconversation.ID,
                    replyId = rconversation.ReplyId,
                    participantId = rconversation.ParticipantId,
                    userName = rconversation.UserName,
                    TopicName = rconversation.Name,
                    firstName = rconversation.FirstName,
                    Message = rconversation.IsMobile == 1 ? (object)rconversation.Message : rconversation.FileData,
                    To = rconversation.AssignToList,
                    CC = rconversation.AssignCCList,
                    userId = rconversation.UserId,
                    isRead = rconversation.IsRead,
                    emailNotificationId = rconversation.EmailNotificationId,
                    urgent = rconversation.Urgent,
                    addedDate = rconversation.AddedDate,
                    sessionId = rconversation.SessionId,
                    isAllowParticipants = rconversation.IsAllowParticipants,
                    

                }),               
                rrgent = conversations.Urgent,
                isAllowParticipants = conversations.IsAllowParticipants,
                dueDate = conversations.DueDate,
                onBehalf = conversations.OnBehalf,
                onBehalfName = conversations.OnBehalfName,
                addedDate = conversations.AddedDate,
                addedByUserID = conversations.AddedByUserID,                
                userId = UserId
            }).ToList();


            return Ok(displayResult);
        }      
        [HttpGet("GetOnReplyList")]
        public async Task<IActionResult> GetOnReplyList(long Id, long UserId)
        {
            var result = await _mediator.Send(new OnReplyConversation(Id, UserId));
            return Ok(result);
        }
        [HttpPost("OnSubmitReply")]
        public async Task<IActionResult> OnSubmitReply(EmailConversations emailConversations)
        {
            if(emailConversations.AssigntoIds != null)
            {
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

                var createReq = new CreateEmailCoversation
                {
                    TopicID = emailConversations.TopicID,
                    FileData = emailConversations.FileData,
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

                var result = await _mediator.Send(createReq);
                return Ok(result);
            }
            else
            {
                return BadRequest("Your request is invalid. Please check your Assign To.");
            }
          
        }
    }
}
