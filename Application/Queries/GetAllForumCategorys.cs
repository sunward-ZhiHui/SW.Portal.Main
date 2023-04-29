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
}
