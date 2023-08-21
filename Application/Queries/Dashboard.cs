using Application.Queries.Base;
using Application.Response;
using Core.Entities;
using MediatR;
 

namespace Application.Queries
{
    public class Dashboard : IRequest<List<EmailScheduler>>
    {
        public string SearchString { get; set; }
    }

    public class GetEmailSchedulerList : PagedRequest, IRequest<List<EmailScheduler>>
    {
        public long UserId { get; private set; }
        public GetEmailSchedulerList(long UserId)
        {
            this.UserId = UserId;
        }
    } 
}
