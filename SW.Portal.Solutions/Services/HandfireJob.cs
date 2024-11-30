using Quartz;
using MediatR;
using Application.Queries;
using DevExpress.Blazor.Popup.Internal;
namespace SW.Portal.Solutions.Services
{
    public class HandfireJob : IJob
    {
        private readonly IMediator _mediator;
        public HandfireJob(IMediator mediator)
        {

            _mediator = mediator;

        }
        public async Task Execute(IJobExecutionContext context)
        {
            var result1 = await _mediator.Send(new GetJobScheduleQuery());
            var result = await _mediator.Send(new GetJobScheduleNavFuctionQuery());
            // Your job logic here
            Console.WriteLine("Handfire job is executing at " + DateTime.Now);
           // return Task.CompletedTask;
        }
    }
}
