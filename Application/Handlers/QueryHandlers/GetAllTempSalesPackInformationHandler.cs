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
    public class GetAllTempSalesPackInformationHandler : IRequestHandler<GetAllTempSalesPackInformationQuery, List<TempSalesPackInformationReportModel>>
    {
        private readonly ITempSalesPackInformationQueryRepository _queryRepository;
        public GetAllTempSalesPackInformationHandler(ITempSalesPackInformationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<TempSalesPackInformationReportModel>> Handle(GetAllTempSalesPackInformationQuery request, CancellationToken cancellationToken)
        {
            return (List<TempSalesPackInformationReportModel>)await _queryRepository.GetTempSalesPackInformationReport();
        }
    }
}
