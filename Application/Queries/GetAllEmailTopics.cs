using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using Core.Repositories.Query;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllEmailTopics : PagedRequest, IRequest<List<EmailTopics>>
    {
        public string SearchString { get; set; }
    }
    public class UpdateTopicArchive : EmailTopics, IRequest<long>
    {
    }
    public class UpdateTopicGroupArchive : EmailTopics, IRequest<long>
    {
    }
    public class UpdateTopicUnArchive : EmailTopics, IRequest<long>
    {
    }

    public class GetActivityEmailTopics : PagedRequest, IRequest<List<ActivityEmailTopics>>
    {
        public string SearchString { get; set; }
    }
    public class GetActivityEmailList : PagedRequest, IRequest<List<ActivityEmailTopics>>
    {
        public Guid SessionId { get; private set; }
        public GetActivityEmailList(Guid SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetActivityEmailDocList : PagedRequest, IRequest<List<ActivityEmailTopics>>
    {
        public Guid SessionId { get; private set; }
        public GetActivityEmailDocList(Guid SessionId)
        {
            this.SessionId = SessionId;
        }
    }

    public class GetByActivityEmailSession : PagedRequest, IRequest<List<ActivityEmailTopics>>
    {
        public Guid EmailTopicSessionId { get; private set; }
        public GetByActivityEmailSession(Guid EmailSessionId)
        {
            this.EmailTopicSessionId = EmailSessionId;
        }
    }
    public class GetUserEmailTopics : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetUserEmailTopics(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class GetRequestEmailToCCList : PagedRequest, IRequest<List<RequestEmail>>
    {
        public long ConId { get; private set; }
        public GetRequestEmailToCCList(long conId)
        {
            this.ConId = conId;
        }
    }
    public class GetRequestEmailToOnlyList : PagedRequest, IRequest<List<RequestEmail>>
    {
        public long ConId { get; private set; }
        public GetRequestEmailToOnlyList(long conId)
        {
            this.ConId = conId;
        }
    }    
    public class GetListBySession : PagedRequest, IRequest<List<EmailTopics>>
    {
        public string SessionId { get; private set; }
        public GetListBySession(string SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetListByIdList : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long ID { get; private set; }
        public GetListByIdList(long Id)
        {
            this.ID = Id;
        }
    }
    
    public class GetEmailTopicTo : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetEmailTopicTo(long UserId,string searchTxt, int pageNumber, int pageSize)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
    public class GetEmailTopicToSearch : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetEmailTopicToSearch(string SearchTxt,long UserId)
        {
            this.UserId = UserId;
            this.SearchTxt = SearchTxt;
        }
    }
    
    public class SetPinEmailTopicTo : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public long UserId { get;set; }
        public SetPinEmailTopicTo(long Id, long UserId)
        {
            this.ID = Id;
            this.UserId = UserId;
        }
    }
    public class UnSetPinEmailTopicTo : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public long UserId { get; set; }
        public UnSetPinEmailTopicTo(long Id, long UserId)
        {
            this.ID = Id;
            this.UserId = UserId;
        }
    }
    public class UpdateMarkasRead : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public UpdateMarkasRead(long Id)
        {
            this.ID = Id;
        }
    }
    public class UpdateMarkasAllRead : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public long UserId { get; private set; }
        public UpdateMarkasAllRead(long Id, long userId)
        {
            this.ID = Id;
            UserId = userId;
        }
    }
    public class UpdateMarkasunRead : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public UpdateMarkasunRead(long Id)
        {
            this.ID = Id;
        }
    }
    public class GetEmailTopicAll : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetEmailTopicAll(long UserId, string searchTxt,int pageNumber,int pageSize)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
    public class GetEmailTopicAllSearch : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetEmailTopicAllSearch(string searchTxt,long UserId)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    
    public class GetEmailTopicHome : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetEmailTopicHome(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class GetEmailMasterSearchAll : EmailSearch, IRequest<List<EmailTopics>>
    {       
       
    }
    public class GetEmailAdminSearchAll : EmailSearch, IRequest<List<EmailTopics>>
    {

    }
    public class GetEmailTopicCC : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetEmailTopicCC(long UserId,string searchTxt)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetEmailTopicCCSearch : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetEmailTopicCCSearch(string searchTxt,long UserId)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetByIdEmailTopicTo : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long ID { get; private set; }
        public GetByIdEmailTopicTo(long topicId)
        {
            this.ID = topicId;
        }
    }
    public class GetByIdEmailTopicCC : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long ID { get; private set; }
        public GetByIdEmailTopicCC(long topicId)
        {
            this.ID = topicId;
        }
    }
    public class GetByIdEmailUserGroupTo : PagedRequest, IRequest<List<long>>
    {
        public long ID { get; private set; }
        public GetByIdEmailUserGroupTo(long topicId)
        {
            this.ID = topicId;
        }
    }
    public class GetByIdEmailUserGroupCC : PagedRequest, IRequest<List<long>>
    {
        public long ID { get; private set; }
        public GetByIdEmailUserGroupCC(long topicId)
        {
            this.ID = topicId;
        }
    }
    public class GetEmailTopicDraft : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetEmailTopicDraft(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class DeleteEmailTopicDraft : PagedRequest, IRequest<long>
    {
        public long TopicId { get; private set; }
        public DeleteEmailTopicDraft(long TopicId)
        {
            this.TopicId = TopicId;
        }
    }
    public class GetSubEmailTopicTo : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSubEmailTopicTo(long ID,long UserId, string searchTxt)
        {
            this.TopicId = ID;
            this.UserId = UserId;
            SearchTxt = searchTxt;
        }
    }
    public class GetSubEmailSearchAll : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSubEmailSearchAll(long ID, long UserId,string SearchTxt)
        {
            this.TopicId = ID;
            this.UserId = UserId;
            this.SearchTxt = SearchTxt;
        }
    }
    public class GetSubEmailTopicAll : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSubEmailTopicAll(long ID, long UserId, string searchTxt)
        {
            this.TopicId = ID;
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetSubAdminEmailAll : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSubAdminEmailAll(long ID, long UserId, string searchTxt)
        {
            this.TopicId = ID;
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetSubEmailTopicHome : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetSubEmailTopicHome(long ID, long UserId)
        {
            this.TopicId = ID;
            this.UserId = UserId;
        }
    }
    public class GetSubEmailTopicCC : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSubEmailTopicCC(long ID, long UserId, string searchTxt)
        {
            this.TopicId = ID;
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetSubEmailTopicSent : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSubEmailTopicSent(long ID, long UserId, string searchTxt)
        {
            this.TopicId = ID;
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
   
    public class GetSentTopic : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSentTopic(long UserId,string searchTxt)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetSentTopicSearch : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string SearchTxt { get; private set; }
        public GetSentTopicSearch(string searchTxt,long UserId)
        {
            this.UserId = UserId;
            this.SearchTxt = searchTxt;
        }
    }
    public class GetEmailParticipantsList : PagedRequest, IRequest<List<EmailParticipant>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetEmailParticipantsList(long topicId,long userId)
        {
            this.TopicId = topicId;
            this.UserId = userId;

        }
    }
    public class GetEmailGroupParticipantsList : PagedRequest, IRequest<List<EmailConversationAssignToUserGroup>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetEmailGroupParticipantsList(long topicId, long userId)
        {
            this.TopicId = topicId;
            this.UserId = userId;

        }
    }
    public class GetConversationParticipantsList : PagedRequest, IRequest<List<EmailParticipant>>
    {
        public long ConversationId { get; private set; }
        public GetConversationParticipantsList(long ConversationId)
        {
            this.ConversationId = ConversationId;
        }
    }
    public class GetConversationGroupParticipantsList : PagedRequest, IRequest<List<EmailConversationAssignToUserGroup>>
    {
        public long ConversationId { get; private set; }
        public GetConversationGroupParticipantsList(long ConversationId)
        {
            this.ConversationId = ConversationId;
        }
    }
    public class GetParticipantbysessionidList : PagedRequest, IRequest<List<EmailParticipant>>
    {
        public Guid sessionId { get; private set; }
        public GetParticipantbysessionidList(Guid sessionId)
        {
            this.sessionId = sessionId;
        }
    }
    
    public class CreateEmailTopicParticipant : TopicParticipant, IRequest<long>
    {
        
    }
    public class CreateActivityEmail : ActivityEmailTopics, IRequest<long>
    {

    }
    public class UpdateActivityEmail : ActivityEmailTopics, IRequest<long>
    {

    }
    public class UpdateEmailTopicDueDate : EmailTopics, IRequest<long>
    {       
    }
    public class UpdateUserTag : EmailActivityCatgorys, IRequest<long>
    {
    }
    public class CreateUserTag : EmailActivityCatgorys, IRequest<long>
    {
    }

    public class UpdateEmailTimelineEvent : EmailTimelineEvent, IRequest<long>
    {
    }
    public class CreateEmailTimelineEvent : EmailTimelineEvent, IRequest<long>
    {
    }
    public class GetTimelineEventList : PagedRequest, IRequest<List<EmailTimelineEvent>>
    {
        public long DocumentId { get; private set; }
        public GetTimelineEventList(long DocumentId)
        {
            this.DocumentId = DocumentId;
        }
    }

    public class CreateEmailDocFileProfileType : Documents, IRequest<long>
    {
    }
    public class UpdateEmailDmsDocId : Documents, IRequest<long>
    {
        public long DocumentId { get; private set; }
        public long EmailToDMS { get; private set; }
        public UpdateEmailDmsDocId(long DocId, long EmailToDMS)
        {
            this.DocumentId = DocId;
            this.EmailToDMS = EmailToDMS;
        }
    }
    

    public class UpdateEmailTopicSubjectDueDate : EmailConversations, IRequest<long>
    {
    }
    public class InsertEmailDueDateHistory: EmailConversations,IRequest<long>
    {

    }
    public class GetEmailDueDateHistory : PagedRequest, IRequest<List<EmailDueDateHistory>>
    {
        public long ID { get; private set; }
        public GetEmailDueDateHistory(long ID)
        {
            this.ID = ID;
        }
    }
    public class UpdateEmailSubjectName : EmailConversations, IRequest<long>
    {
    }
    public class UpdateEmailTopicClosed : EmailTopics, IRequest<long>
    {       
    }

    public class GetByIdEmailTopics : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long ID { get; private set; }
        public GetByIdEmailTopics(long ID)
        {
            this.ID = ID;
        }
    }
    public class GetAllCreateEmailDocumentLst : PagedRequest, IRequest<List<Documents>>
    {
        public Guid SessionId { get; private set; }
        public GetAllCreateEmailDocumentLst(Guid sessionId)
        {
            this.SessionId = sessionId;
        }

    }
    public class GetDynamicFormDoc : PagedRequest, IRequest<List<Documents>>
    {
        public Guid SessionId { get; private set; }
        public GetDynamicFormDoc(Guid sessionId)
        {
            this.SessionId = sessionId;
        }

    }
    public class GetDynamicFormEmailSection : PagedRequest, IRequest<List<DynamicFormSection>>
    {
        public Guid SessionId { get; private set; }
        public GetDynamicFormEmailSection(Guid sessionId)
        {
            this.SessionId = sessionId;
        }
    }
    public class GetUserPermissionSection : PagedRequest, IRequest<List<DynamicFormSectionSecurity>>
    {
        public long Id { get; private set; }
        public GetUserPermissionSection(long Id)
        {
            this.Id = Id;
        }
    }
    public class GetDynamicFormName : PagedRequest, IRequest<List<DynamicFormData>>
    {
        public Guid SessionId { get; private set; }
        public GetDynamicFormName(Guid sessionId)
        {
            this.SessionId = sessionId;
        }

    }

    public class GetPATypeDocLst : PagedRequest, IRequest<List<Documents>>
    {
        public long Id { get; private set; }
        public string Type { get; private set; }
        public GetPATypeDocLst(long Id, string type)
        {
            this.Id = Id;
            Type = type;
        }

    }
    public class DeleteDocumentFileQuery : Documents, IRequest<long>
    {
        public long ID { get; set; }

        public DeleteDocumentFileQuery(long Id)
        {
            this.ID = Id;
        }
    }
}
