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
    public class GetAllStockInformationMasterHandler : IRequestHandler<GetAllStockInformationMasterQuery, List<StockInformationMaster>>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetAllStockInformationMasterHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<StockInformationMaster>> Handle(GetAllStockInformationMasterQuery request, CancellationToken cancellationToken)
        {
            return (List<StockInformationMaster>)await _queryRepository.GetAllByAsync();
        }
    }
    public class InsertOrUpdateStockInformationMasterHandler : IRequestHandler<InsertOrUpdateStockInformationMaster, StockInformationMaster>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public InsertOrUpdateStockInformationMasterHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<StockInformationMaster> Handle(InsertOrUpdateStockInformationMaster request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateStockInformationMaster(request);

        }
    }

    public class DeleteStockInformationMasterHandler : IRequestHandler<DeleteStockInformationMaster, StockInformationMaster>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public DeleteStockInformationMasterHandler(IStockInformationMasterQueryRepository  queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<StockInformationMaster> Handle(DeleteStockInformationMaster request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteStockInformationMaster(request.StockInformationMaster);
        }

    }
}
