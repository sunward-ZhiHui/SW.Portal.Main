using Quartz;
using MediatR;
using Application.Queries;
using DevExpress.Blazor.Popup.Internal;
using Core.Repositories.Query;
namespace SW.Portal.Solutions.Services
{
    public class HandfireJob : IJob
    {
        private readonly IMediator _mediator;
        private readonly IHandFireJobQueryRepository _IHandFireJobQueryRepository;
        public HandfireJob(IMediator mediator, IHandFireJobQueryRepository handFireJobQueryRepository)
        {

            _mediator = mediator;
            _IHandFireJobQueryRepository = handFireJobQueryRepository;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            await _IHandFireJobQueryRepository.InsertHandFireJob("StartNavVendor");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("NavVendor"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndNavVendor");

            await _IHandFireJobQueryRepository.InsertHandFireJob("StartNavItems");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("NavItems"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndNavItems");

            await _IHandFireJobQueryRepository.InsertHandFireJob("StartFinishedProdOrder");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("FinishedProdOrder"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndFinishedProdOrder");

            await _IHandFireJobQueryRepository.InsertHandFireJob("StartItemBatchInfo");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("ItemBatchInfo"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndItemBatchInfo");

            await _IHandFireJobQueryRepository.InsertHandFireJob("StartNavprodOrder");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("NavprodOrder"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndFinishedProdOrder");

            await _IHandFireJobQueryRepository.InsertHandFireJob("StartRawMatItem");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("RawMatItem"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndRawMatItem");

            await _IHandFireJobQueryRepository.InsertHandFireJob("StartRawMatPurch");
            await _mediator.Send(new GetJobScheduleNavFuctionQuery("RawMatPurch"));
            await _IHandFireJobQueryRepository.InsertHandFireJob("EndRawMatPurch");
        }
    }
}
