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
    public class GetAllSalesOrderHandler : IRequestHandler<GetAllSalesOrderQuery, List<SalesOrderModel>>
    {
        private readonly ISalesOrderQueryRepository _queryRepository;
        public GetAllSalesOrderHandler(ISalesOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SalesOrderModel>> Handle(GetAllSalesOrderQuery request, CancellationToken cancellationToken)
        {
            return (List<SalesOrderModel>)await _queryRepository.GetAllByAsync();
        }
    }
    public class InsertOrUpdateSalesOrderHandler : IRequestHandler<InsertOrUpdateSalesOrder, SalesOrderModel>
    {
        private readonly ISalesOrderQueryRepository _queryRepository;
        public InsertOrUpdateSalesOrderHandler(ISalesOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<SalesOrderModel> Handle(InsertOrUpdateSalesOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateSalesOrder(request);

        }
    }
    public class DeleteSalesOrderHandler : IRequestHandler<DeleteSalesOrder, SalesOrderModel>
    {
        private readonly ISalesOrderQueryRepository _queryRepository;
        public DeleteSalesOrderHandler(ISalesOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<SalesOrderModel> Handle(DeleteSalesOrder request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteSalesOrder(request.SalesOrderModel);
        }

    }
}
