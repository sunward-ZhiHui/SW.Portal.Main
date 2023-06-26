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
    public class GetUserEmailTopics : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetUserEmailTopics(long UserId)
        {
            this.UserId = UserId;
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
    public class GetEmailTopicCC : PagedRequest, IRequest<List<EmailTopics>>
    {
        public long UserId { get; private set; }
        public GetEmailTopicCC(long UserId)
        {
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
}
