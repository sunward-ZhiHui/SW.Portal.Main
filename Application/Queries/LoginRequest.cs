using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class LoginRequest : IRequest<ApplicationUser>
    {
        public string LoginID { get; set; }
        public string Password { get; set; }
    }
   
}
