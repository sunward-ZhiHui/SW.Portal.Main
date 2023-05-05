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
    public class GetListCategory : IRequest<List<ForumCategorys>>
    {
        public long TypeId { get; private set; }

        public GetListCategory(long typeId)
        {
            this.TypeId = typeId;
        }
    }
}
