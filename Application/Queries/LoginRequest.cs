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
    public class UserLoginStatus : IRequest<ApplicationUser>
    {
        public string LoginID { get; set; }        
    }

    public class UpdateOutlookLoginQuery : ApplicationUser, IRequest<long>
    {
    }

    public class UpdateUserPasswordRequest : IRequest<ApplicationUser>
    {
        public long UserID { get; set; }
        public string LoginID { get; set; }
        public string NewPassword { get; set; }
        public string OldPassword { get; set; }
    }
    public class ResetUserPasswordRequest : IRequest<ApplicationUser>
    {
        public string LoginID { get; set; }
        public string NewPassword { get; set; }
    }
    public class UnsetLockedRequest : IRequest<ApplicationUser>
    {
        public string LoginID { get; set; }
        public string NewPassword { get; set; }
        public bool? Locked { get; set; }
    }
    public class ActiveRequest : IRequest<ApplicationUser>
    {
        public string LoginID { get; set; }
       
    }
    public class InActiveRequest : IRequest<ApplicationUser>
    {
        public string LoginID { get; set; }

    }
}
