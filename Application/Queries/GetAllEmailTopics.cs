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

    public class GetActivityEmailTopics : PagedRequest, IRequest<List<ActivityEmailTopics>>
    {
        public string SearchString { get; set; }
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
    public class GetListBySession : PagedRequest, IRequest<List<EmailTopics>>
    {
        public string SessionId { get; private set; }
        public GetListBySession(string SessionId)
        {
            this.SessionId = SessionId;
        }
    }
    public class GetEmailTopicTo : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetEmailTopicTo(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class SetPinEmailTopicTo : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public SetPinEmailTopicTo(long Id)
        {
            this.ID = Id;
        }
    }
    public class UnSetPinEmailTopicTo : PagedRequest, IRequest<long>
    {
        public long ID { get; private set; }
        public UnSetPinEmailTopicTo(long Id)
        {
            this.ID = Id;
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
        public GetEmailTopicAll(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class GetEmailMasterSearchAll : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public string? MasterSearch { get; private set; }
        public GetEmailMasterSearchAll(long UserId, string masterSearch)
        {
            this.UserId = UserId;
            MasterSearch = masterSearch;
        }
    }
    public class GetEmailTopicCC : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetEmailTopicCC(long UserId)
        {
            this.UserId = UserId;
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
        public GetSubEmailTopicTo(long ID,long UserId)
        {
            this.TopicId = ID;
            this.UserId = UserId;
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
        public GetSubEmailTopicAll(long ID, long UserId)
        {
            this.TopicId = ID;
            this.UserId = UserId;
        }
    }
    public class GetSubEmailTopicCC : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetSubEmailTopicCC(long ID, long UserId)
        {
            this.TopicId = ID;
            this.UserId = UserId;
        }
    }
    public class GetSubEmailTopicSent : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long TopicId { get; private set; }
        public long UserId { get; private set; }
        public GetSubEmailTopicSent(long ID, long UserId)
        {
            this.TopicId = ID;
            this.UserId = UserId;
        }
    }
   
    public class GetSentTopic : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetSentTopic(long UserId)
        {
            this.UserId = UserId;
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
    public class GetConversationParticipantsList : PagedRequest, IRequest<List<EmailParticipant>>
    {
        public long ConversationId { get; private set; }
        public GetConversationParticipantsList(long ConversationId)
        {
            this.ConversationId = ConversationId;
        }
    }
    public class CreateEmailTopicParticipant : TopicParticipant, IRequest<long>
    {
        
    }
    public class UpdateEmailTopicDueDate : EmailTopics, IRequest<long>
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
     public class DeleteDocumentFileQuery : Documents, IRequest<long>
    {
        public long ID { get; set; }

        public DeleteDocumentFileQuery(long Id)
        {
            this.ID = Id;
        }
    }
}
