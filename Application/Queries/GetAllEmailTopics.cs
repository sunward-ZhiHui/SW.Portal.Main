using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllEmailTopics : PagedRequest, IRequest<List<EmailTopics>>
    {
        public string SearchString { get; set; }
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
    public class GetEmailTopicAll : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetEmailTopicAll(long UserId)
        {
            this.UserId = UserId;
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
        public GetEmailParticipantsList(long topicId)
        {
            this.TopicId = topicId;
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
