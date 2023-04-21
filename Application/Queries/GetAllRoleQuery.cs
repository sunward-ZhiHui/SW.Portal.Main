using Application.Queries.Base;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllRoleQuery : PagedRequest, IRequest<List<ApplicationRole>>
    {
        public string SearchString { get; set; }
    }
}
