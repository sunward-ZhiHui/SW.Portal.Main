using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllForumTypes : PagedRequest, IRequest<List<ForumTypes>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllForumTypesUsers : PagedRequest, IRequest<List<ForumTypes>>
    {
        public string SearchString { get; set; }
    }
    public class GetTypeList : PagedRequest, IRequest<List<ForumTypes>>
    {
        public long ID { get; private set; }
        public GetTypeList(long Id)
        {
            this.ID = Id;
        }
    }
}
