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
}
