using Application.Queries;
using Core.Entities;
using Core.Entities.Views;
using Core.Repositories.Query;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllProductionActivityAppLineReportHandler : IRequestHandler<GetAllProductionActivityAppLineReport, List<view_ProductionActivityAppLineReport>>
    {
        private readonly IProductionActivityAppLineReportQueryRepository _queryRepository;
        public GetAllProductionActivityAppLineReportHandler(IProductionActivityAppLineReportQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<view_ProductionActivityAppLineReport>> Handle(GetAllProductionActivityAppLineReport request, CancellationToken cancellationToken)
        {
            return (List<view_ProductionActivityAppLineReport>)await _queryRepository.GetAllAsync();
        }
    }
    public class GetAllProductionActivityAppLineFilterReportHandler : IRequestHandler<GetAllProductionActivityAppLineFilterReport, List<view_ProductionActivityAppLineReport>>
    {
        private readonly IProductionActivityAppLineReportQueryRepository _queryRepository;
        public GetAllProductionActivityAppLineFilterReportHandler(IProductionActivityAppLineReportQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<view_ProductionActivityAppLineReport>> Handle(GetAllProductionActivityAppLineFilterReport request, CancellationToken cancellationToken)
        {
            return (List<view_ProductionActivityAppLineReport>)await _queryRepository.GetAllFilterAsync(request.CompanyId,request.FromDate, request.ToDate);
        }
    }
    public class GetAllDocumentListHandler : IRequestHandler<GetAllDocumentList, List<Documents>>
    {
        private readonly IProductionActivityAppLineReportQueryRepository _rolepermissionQueryRepository;

        public GetAllDocumentListHandler(IProductionActivityAppLineReportQueryRepository rolepermissionQueryRepository)
        {

            _rolepermissionQueryRepository = rolepermissionQueryRepository;
        }
        public async Task<List<Documents>> Handle(GetAllDocumentList request, CancellationToken cancellationToken)
        {
            return (List<Documents>)await _rolepermissionQueryRepository.GetDocumentListAsync(request.sessionId);
        }
    }
}
