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
using static Core.EntityModels.SyrupPlanning;

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

    public class GetSyrupPlanningQueryHandler : IRequestHandler<GetSyrupPlanningQuery, List<SyrupPlanning>>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetSyrupPlanningQueryHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SyrupPlanning>> Handle(GetSyrupPlanningQuery request, CancellationToken cancellationToken)
        {
            return (List<SyrupPlanning>)await _queryRepository.GetSyrupPlannings();
        }
    }
    public class GetSyrupProcessNameListQueryHandler : IRequestHandler<GetSyrupProcessNameListQuery, List<SyrupProcessNameList>>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetSyrupProcessNameListQueryHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SyrupProcessNameList>> Handle(GetSyrupProcessNameListQuery request, CancellationToken cancellationToken)
        {
            return (List<SyrupProcessNameList>)await _queryRepository.GetSyrupProcessNameList(request.DynamicFormDataID);
        }
    }
    public class GetSyrupFillingListQueryHandler : IRequestHandler<GetSyrupFillingListQuery, List<SyrupFilling>>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetSyrupFillingListQueryHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SyrupFilling>> Handle(GetSyrupFillingListQuery request, CancellationToken cancellationToken)
        {
            return (List<SyrupFilling>)await _queryRepository.GetSyrupFillingList(request.DynamicFormDataID);
        }
    }
    public class GetSyrupOtherProcessListQueryHandler : IRequestHandler<GetSyrupOtherProcessListQuery, List<SyrupOtherProcess>>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetSyrupOtherProcessListQueryHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<SyrupOtherProcess>> Handle(GetSyrupOtherProcessListQuery request, CancellationToken cancellationToken)
        {
            return (List<SyrupOtherProcess>)await _queryRepository.GetSyrupOtherProcessList();
        }
    }
    


    public class GetSyrupSimplexDataListHandler : IRequestHandler<GetSyrupSimplexDataList, SyrupPlanning>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetSyrupSimplexDataListHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<SyrupPlanning> Handle(GetSyrupSimplexDataList request, CancellationToken cancellationToken)
        {
            return (SyrupPlanning)await _queryRepository.SelectSyrupSimplexDataList(request.DynamicFormDataID);
        }
    }
    public class GetSyruppreparationDataListHandler : IRequestHandler<GetSyruppreparationDataList, SyrupPlanning>
    {
        private readonly IStockInformationMasterQueryRepository _queryRepository;
        public GetSyruppreparationDataListHandler(IStockInformationMasterQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<SyrupPlanning> Handle(GetSyruppreparationDataList request, CancellationToken cancellationToken)
        {
            return (SyrupPlanning)await _queryRepository.SelectSyruppreparationDataList(request.DynamicFormDataID);
        }
    }


}
