using Application.Queries.Base;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllApplicationUserQuery : PagedRequest, IRequest<List<ApplicationUser>>
    {
        public string SearchString { get; set; }
    }    
     public class AddPushTokenID : PagedRequest, IRequest<String>
    {
        public string LoginId { get; set; }
        public string DeviceType { get; set; }
        public string TokenID { get; set; }
        public AddPushTokenID(string loginId,string deviceType,string tokenId)
        {
            this.LoginId = loginId;
            this.DeviceType = deviceType;
            this.TokenID = tokenId;
           
        }
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
