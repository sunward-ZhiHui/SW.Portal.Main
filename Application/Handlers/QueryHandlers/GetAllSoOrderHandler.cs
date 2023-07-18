using Application.Queries;
using Application.Response;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModel;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Linq;

namespace CMS.Application.Handlers.QueryHandlers
{
    public class GetAllSoOrderHandler : IRequestHandler<GetAllSoOrder, List<View_SoSalesOrder>>
    {
        private readonly ISoSalesOrderQueryRepository _queryRepository;
        public GetAllSoOrderHandler(ISoSalesOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_SoSalesOrder>> Handle(GetAllSoOrder request, CancellationToken cancellationToken)
        {
            return (List<View_SoSalesOrder>)await _queryRepository.GetAllAsync();
        }
    }

    public class GetAllSoOrderBySessionHandler : IRequestHandler<GetAllSoOrderBySession, View_SoSalesOrder>
    {
        private readonly ISoSalesOrderQueryRepository _queryRepository;
        public GetAllSoOrderBySessionHandler(ISoSalesOrderQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<View_SoSalesOrder> Handle(GetAllSoOrderBySession request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetAllBySessionAsync(request.SesionId);
        }
    }
}
