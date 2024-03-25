using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllEmailConversation : PagedRequest, IRequest<List<EmailConversations>>
    {
        public string SearchString { get; set; }
    }
    public class GetListByConversationSession : PagedRequest, IRequest<List<EmailConversations>>
    {
        public string SessionId { get; private set; }
        public GetListByConversationSession(string SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class CreateEmailCoversation : EmailConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class EditEmailConversation : EmailConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteEmailConversation : EmailConversations, IRequest<long>
    {
        public string SearchString { get; set; }
    }
    public class DeleteEmailParticipant : TopicParticipant, IRequest<long>
    {        
    }
    public class GetAllEmailParticipantListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; private set; }
        public GetAllEmailParticipantListQuery(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetAddConversationPListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long ConversationId { get; private set; }
        public GetAddConversationPListQuery(long ConversationId)
        {
            this.ConversationId = ConversationId;
        }
    }
    
    public class GetEmailFullDiscussionList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; private set; }
        public GetEmailFullDiscussionList(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetEmailDiscussionList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string Option { get; private set; }
        public GetEmailDiscussionList(long TopicId,long UserId,string option)
        {
            this.TopicId = TopicId;
            this.UserId = UserId;
            this.Option = option;
        }
    }
    public class GetReplyDiscussionList : IRequest<List<EmailConversations>>
    {
        public long ReplyId { get; private set; }
       
        public GetReplyDiscussionList(long replyId)
        {           
            this.ReplyId = replyId;
        }
    }
    public class GetDemoEmailFileDataList : PagedRequest, IRequest<List<EmailConversations>>
    {  
    }
    public class GetDemoUpdateEmailFileDataList : PagedRequest, IRequest<long>
    {
        public long id { get; private set; }
        public byte[] fileData { get; private set; }
        public GetDemoUpdateEmailFileDataList(long id, byte[] fileData)
        {
            this.id = id;
            this.fileData = fileData;
        }
    }
    public class GetEmailValidUserList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetEmailValidUserList(long TopicId, long UserId)
        {
            this.TopicId = TopicId;
            this.UserId = UserId;
        }
    }
    public class OnReplyConversation : PagedRequest, IRequest<OnReplyEmail>
    {
        public long ID { get; private set; }
        public long UserId { get; private set; }
        public OnReplyConversation(long Id,long UserId)
        {
            this.ID = Id;
            this.UserId= UserId;
        }
    }
    public class OnReplyConversationTopic : PagedRequest, IRequest<OnReplyEmailTopic>
    {
        public long ID { get; private set; }
        public long UserId { get; private set; }
        public OnReplyConversationTopic(long Id, long userId)
        {
            this.ID = Id;
            this.UserId = userId;
        }
    }
    public class OnReplyDropDown : PagedRequest, IRequest<OnReplyEmailTopic>
    {
        public string ToIds { get; set; }
        public string CcIds { get; set; }
        public OnReplyDropDown(string ToIds, string CcIds)
        {
            this.CcIds = ToIds;
            this.ToIds = CcIds;
        }
    }
    public class GetEmailConversationList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long ID { get; private set; }
        public GetEmailConversationList(long Id)
        {
            this.ID = Id;
        }
    }
    public class GetTopConversationList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; private set; }
        public GetTopConversationList(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetEmailReplyDiscussionList : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetEmailReplyDiscussionList(long TopicId,long UserId)
        {
            this.TopicId = TopicId;
            this.UserId = UserId;
        }
    }
    public class GetEmailTopicDocList : PagedRequest, IRequest<List<Documents>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string Option { get; private set; }
        public GetEmailTopicDocList(long TopicId, long userId, string option)
        {
            this.TopicId = TopicId;
            UserId = userId;
            Option = option;
        }
    }
    public class GetSubEmailTopicDocList : PagedRequest, IRequest<List<Documents>>
    {
        public long ConversationId { get; private set; }
        public GetSubEmailTopicDocList(long ConvId)
        {
            this.ConversationId = ConvId;
        }
    }
    public class GetEmailAssignToList : PagedRequest, IRequest<List<EmailAssignToList>>
    {
        public long TopicId { get; set; }
        public GetEmailAssignToList(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetByEmailTopicIDPList : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; set; }
        public GetByEmailTopicIDPList(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    
    public class GetByConvasationPList : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; set; }
        public long ConvasationId { get; set; }
        public GetByConvasationPList(long convId)
        {            
            this.ConvasationId = convId;
        }
    }
    public class GetByConvasationPUserGroupList : PagedRequest, IRequest<List<long>>
    {        
        public long ConvasationId { get; set; }
        public GetByConvasationPUserGroupList(long convId)
        {
            this.ConvasationId = convId;
        }
    }
    
    public class GetByConvasationTopicIDPList : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; set; }
        public long ConvasationId { get; set; }
        public GetByConvasationTopicIDPList(long convId,long topicId)
        {
            this.TopicId = topicId;
            this.ConvasationId = convId;
        }
    }

    public class GetByConvasationTIDPList : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long TopicId { get; set; }        
        public GetByConvasationTIDPList(long topicId)
        {
            this.TopicId = topicId;            
        }
    }
    public class GetAllConvAssToListQuery : PagedRequest, IRequest<List<ViewEmployee>>
    {
        public long EmployeeID { get; set; }
        public GetAllConvAssToListQuery(long conversationId)
        {
            this.EmployeeID = conversationId;
        }
    }
    public class GetByIdConversation : PagedRequest, IRequest<EmailConversations>
    {
        public long Id { get; set; }
        public GetByIdConversation(long id)
        {
            this.Id = id;
        }
    }   

    public class GetUserTokenListQuery : PagedRequest, IRequest<List<UserNotification>>
    {
        public long UserId { get; set; }
        public GetUserTokenListQuery(long userId)
        {
            this.UserId = userId;
        }
    }

    public class GetEmailTopicToList : PagedRequest, IRequest<List<EmailTopicTo>>
    {
        public long TopicId { get; set; }
        public GetEmailTopicToList(long topicId)
        {
            this.TopicId = topicId;
        }
    }
    public class GetEmailConversationAssignTo : PagedRequest, IRequest<List<EmailConversationAssignTo>>
    {
        public long ConversationId { get; set; }
        public GetEmailConversationAssignTo(long Id)
        {
            this.ConversationId = Id;
        }
    }
    public class GetEmailConversationTId : PagedRequest, IRequest<List<EmailConversations>>
    {
        public long TopicId { get; set; }
        public GetEmailConversationTId(long topicId)
        {
            this.TopicId = topicId;
        }
    }

    public class GetEmailConversationAssignCC : PagedRequest, IRequest<List<EmailConversationAssignTo>>
	{
		public long ConversationId { get; set; }
		public GetEmailConversationAssignCC(long Id)
		{
			this.ConversationId = Id;
		}
	}

    public class GetAssignCCUserGroup : PagedRequest, IRequest<List<EmailConversationAssignToUserGroup>>
    {
        public long ConversationId { get; set; }
        public GetAssignCCUserGroup(long Id)
        {
            this.ConversationId = Id;
        }
    }

    

}
