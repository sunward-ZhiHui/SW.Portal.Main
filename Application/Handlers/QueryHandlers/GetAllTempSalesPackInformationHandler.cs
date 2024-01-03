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
    public class GetAllTempSalesPackInformationSyncHandler : IRequestHandler<GetAllTempSalesPackInformationSyncQuery, TempSalesPackInformationReportModel>
    {
        private readonly ITempSalesPackInformationQueryRepository _queryRepository;
        public GetAllTempSalesPackInformationSyncHandler(ITempSalesPackInformationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<TempSalesPackInformationReportModel> Handle(GetAllTempSalesPackInformationSyncQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetTempSalesPackInformationReportSync(request.TempSalesPackInformationReportModel);
        }
    }
    public class GetAllTempSalesPackInformationFactorHandler : IRequestHandler<GetTempSalesPackInformationFactor, List<TempSalesPackInformationFactor>>
    {
        private readonly ITempSalesPackInformationQueryRepository _queryRepository;
        public GetAllTempSalesPackInformationFactorHandler(ITempSalesPackInformationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<TempSalesPackInformationFactor>> Handle(GetTempSalesPackInformationFactor request, CancellationToken cancellationToken)
        {
            return (List<TempSalesPackInformationFactor>)await _queryRepository.GetTempSalesPackInformationFactor(request.Id);
        }
    }
    public class InsertTempSalesPackInformationFactorHandler : IRequestHandler<TempSalesPackInformationFactorQuery, TempSalesPackInformationFactor>
    {
        private readonly ITempSalesPackInformationQueryRepository _queryRepository;
        public InsertTempSalesPackInformationFactorHandler(ITempSalesPackInformationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<TempSalesPackInformationFactor> Handle(TempSalesPackInformationFactorQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertTempSalesPackInformationFactor(request.TempSalesPackInformationFactor);
        }
    }
    public class DeleteTempSalesPackInformationFactorQueryHandler : IRequestHandler<DeleteTempSalesPackInformationFactorQuery, TempSalesPackInformationFactor>
    {
        private readonly ITempSalesPackInformationQueryRepository _queryRepository;
        public DeleteTempSalesPackInformationFactorQueryHandler(ITempSalesPackInformationQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<TempSalesPackInformationFactor> Handle(DeleteTempSalesPackInformationFactorQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteTempSalesPackInformationFactor(request.Id);
        }
    }
}
