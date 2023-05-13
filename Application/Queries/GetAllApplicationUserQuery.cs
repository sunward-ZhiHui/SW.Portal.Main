using Application.Queries.Base;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllApplicationUserQuery : PagedRequest, IRequest<List<ApplicationUser>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllApplicationUserByLoginIDQuery : PagedRequest, IRequest<ApplicationUser>
    {
        public string Name { get; set; }
        public GetAllApplicationUserByLoginIDQuery(string Name)
        {
            this.Name = Name;
        }
    }
}
