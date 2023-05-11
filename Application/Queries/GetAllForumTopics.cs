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
}
