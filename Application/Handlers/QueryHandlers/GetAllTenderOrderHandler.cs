using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllTenderOrderHandler : IRequestHandler<GetAllTenderOrderQuery, List<TenderOrderModel>>
    {
        private readonly ITenderOrdersQueryRepository _queryRepository;
        public GetAllTenderOrderHandler(ITenderOrdersQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<TenderOrderModel>> Handle(GetAllTenderOrderQuery request, CancellationToken cancellationToken)
        {
            return (List<TenderOrderModel>)await _queryRepository.GetAllByAsync();
        }
    }

    public class InsertOrUpdateTenderOrderHandler : IRequestHandler<InsertOrUpdateTenderOrder, TenderOrderModel>
    {
        private readonly ITenderOrdersQueryRepository _queryRepository;
        public InsertOrUpdateTenderOrderHandler(ITenderOrdersQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<TenderOrderModel> Handle(InsertOrUpdateTenderOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateTenderOrder(request);

        }
    }

    public class DeleteTenderOrderHandler : IRequestHandler<DeleteTenderOrder, TenderOrderModel>
    {
        private readonly ITenderOrdersQueryRepository _queryRepository;
        public DeleteTenderOrderHandler(ITenderOrdersQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<TenderOrderModel> Handle(DeleteTenderOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTenderOrder(request.TenderOrderModel);
        }

    }
}
