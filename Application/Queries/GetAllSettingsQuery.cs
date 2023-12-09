using Application.Queries.Base;
using Core.Entities;
using Core.EntityModels;
using MediatR;
 

namespace Application.Queries
{
    public class GetAllSettingsQuery : PagedRequest, IRequest<List<OpenAccessUserLink>>
    {
        public long? OpenAccessUserId { get; set; }
        public GetAllSettingsQuery(long? openAccessUserId)
        {
            this.OpenAccessUserId = openAccessUserId;
        }
    }
    public class DeleteOpenAccessUserLink : PagedRequest, IRequest<OpenAccessUserLink>
    {
        public OpenAccessUserLink OpenAccessUserLink { get; private set; }
        public DeleteOpenAccessUserLink(OpenAccessUserLink openAccessUserLink)
        {
            this.OpenAccessUserLink = openAccessUserLink;
        }
    }
    public class InsertOpenAccessUserLink : PagedRequest, IRequest<OpenAccessUserLink>
    {
        public OpenAccessUserLink OpenAccessUserLink { get; private set; }
        public InsertOpenAccessUserLink(OpenAccessUserLink openAccessUserLink)
        {
            this.OpenAccessUserLink = openAccessUserLink;
        }
    }
    public class GetOpenAccessUser : PagedRequest, IRequest<OpenAccessUser>
    {
        public string? AccessType { get; set; }
        public GetOpenAccessUser(string? accessType)
        {
            this.AccessType = accessType;
        }
    }
    public class GetDMSAccessByUser : PagedRequest, IRequest<OpenAccessUserLink>
    {
        public long? UserId { get; set; }
        public GetDMSAccessByUser(long? userId)
        {
            this.UserId = userId;
        }
    }
    public class GetEmailAccessByUser : PagedRequest, IRequest<OpenAccessUserLink>
    {
        public long? UserId { get; set; }
        public GetEmailAccessByUser(long? userId)
        {
            this.UserId = userId;
        }
    }

}
