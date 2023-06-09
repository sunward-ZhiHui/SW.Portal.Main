using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllForumCategorys : PagedRequest, IRequest<List<ForumCategorys>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllForumCategorysUsers : PagedRequest, IRequest<List<ForumCategorys>>
    {
        public string SearchString { get; set; }
    }
    public class GetListCategory : IRequest<List<ForumCategorys>>
    {
        public long ID { get; private set; }

        public GetListCategory(long id)
        {
            this.ID = id;
        }
    }
}
