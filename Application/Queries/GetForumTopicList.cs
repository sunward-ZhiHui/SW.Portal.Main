using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetForumTopicList : PagedRequest, IRequest<List<ForumTopicsResponse>>
    {
        public string SearchString { get; set; }
    }   
}
