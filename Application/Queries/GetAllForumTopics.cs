using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllForumTopics : PagedRequest, IRequest<List<ForumTopics>>
    {
        public string SearchString { get; set; }
    }
    public class GetUserForumTopics : PagedRequest, IRequest<List<ForumTopics>>
    {
        public long UserId { get; private set; }
        public GetUserForumTopics(long UserId)
        {
            this.UserId = UserId;
        }
    }
    public class GetTreeList : PagedRequest, IRequest<List<ForumTopics>>
    {
        public long UserId { get; private set; }
        public GetTreeList(long UserId)
        {
            this.UserId = UserId;
        }
    }    
    public class GetParticipantsList : PagedRequest, IRequest<List<TopicParticipant>>
    {
        public long TopicId { get; private set; }
        public GetParticipantsList(long topicId)
        {
            this.TopicId = topicId;
        }
    }    
    public class CreateTopicParticipant : TopicParticipant, IRequest<long>
    {
        
    }

    public class GetByIdTopics : PagedRequest, IRequest<List<ForumTopics>>
    {
        public long ID { get; private set; }
        public GetByIdTopics(long ID)
        {
            this.ID = ID;
        }
    }
}
