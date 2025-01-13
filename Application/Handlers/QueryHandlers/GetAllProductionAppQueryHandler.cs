using Application.Queries;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using Core.Repositories.Query.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers.QueryHandlers
{
    public class GetAllProductionAppQueryHandler : IRequestHandler<GetAllProductionActivityAppQuery, List<ProductionActivityApp>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetAllProductionAppQueryHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<ProductionActivityApp>> Handle(GetAllProductionActivityAppQuery request, CancellationToken cancellationToken)
        {
            return (List<ProductionActivityApp>)await _productionactivityappQueryRepository.GetAllAsync(request.companyID);
        }

    }
    public class GetAllNavprodOrderLineHandler : IRequestHandler<GetAllNavprodOrderLine, List<NavprodOrderLineModel>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetAllNavprodOrderLineHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<NavprodOrderLineModel>> Handle(GetAllNavprodOrderLine request, CancellationToken cancellationToken)
        {
            return (List<NavprodOrderLineModel>)await _productionactivityappQueryRepository.GetAllNavprodOrderLineAsync(request.CompanyID, request.Replanrefno);
        }

    }
    public class GetProductActivityCaseLineTemplateItemsHandler : IRequestHandler<GetProductActivityCaseLineTemplateItems, List<ProductActivityCaseLineModel>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetProductActivityCaseLineTemplateItemsHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<ProductActivityCaseLineModel>> Handle(GetProductActivityCaseLineTemplateItems request, CancellationToken cancellationToken)
        {
            return (List<ProductActivityCaseLineModel>)await _productionactivityappQueryRepository.GetProductActivityCaseLineTemplateItemsAsync(request.ManufacturingProcessId, request.CategoryActionId,request.ProdActivityActionChildD);
        }

    }
    public class GetAllProductionAppLocationQueryHandler : IRequestHandler<GetAllProductionActivityLocationAppQuery, ProductionActivityApp>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetAllProductionAppLocationQueryHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<ProductionActivityApp> Handle(GetAllProductionActivityLocationAppQuery request, CancellationToken cancellationToken)
        {
            return await _productionactivityappQueryRepository.GetAllOneLocationAsync(request.LocationName);
        }

    }
    public class CreateProductionAppQueryHandler : IRequestHandler<CreateProductionActivityAppCommand, long>
    {
        private readonly IProductionActivityAppQueryRepository _PPAppLineListQueryRepository;
        public CreateProductionAppQueryHandler(IProductionActivityAppQueryRepository PPAppLineListQueryRepository, IQueryRepository<ProductActivityAppModel> queryRepository)
        {
            _PPAppLineListQueryRepository = PPAppLineListQueryRepository;
        }

        public async Task<long> Handle(CreateProductionActivityAppCommand request, CancellationToken cancellationToken)
        {
            var newlist = await _PPAppLineListQueryRepository.Insert(request);
            return newlist;

        }
    }
    public class GetAllProductionActivityPONumberAppQueryHandler : IRequestHandler<GetAllProductionActivityPONumberAppQuery, List<NavprodOrderLineModel>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappponumberQueryRepository;
        public GetAllProductionActivityPONumberAppQueryHandler(IProductionActivityAppQueryRepository productionactivityappponumberQueryRepository)
        {
            _productionactivityappponumberQueryRepository = productionactivityappponumberQueryRepository;
        }
        public async Task<List<NavprodOrderLineModel>> Handle(GetAllProductionActivityPONumberAppQuery request, CancellationToken cancellationToken)
        {
            return (List<NavprodOrderLineModel>)await _productionactivityappponumberQueryRepository.GetAllAsyncPO(request.companyID);
        }

    }
    public class GetAllProductionActivityFBNumberAppQueryHandler : IRequestHandler<GetAllProductionActivityFPNumberAppQuery, List<NavprodOrderLineModel>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappponumberQueryRepository;
        public GetAllProductionActivityFBNumberAppQueryHandler(IProductionActivityAppQueryRepository productionactivityappponumberQueryRepository)
        {
            _productionactivityappponumberQueryRepository = productionactivityappponumberQueryRepository;
        }
        public async Task<List<NavprodOrderLineModel>> Handle(GetAllProductionActivityFPNumberAppQuery request, CancellationToken cancellationToken)
        {
            return (List<NavprodOrderLineModel>)await _productionactivityappponumberQueryRepository.GetAllFpAsyncPO(request.companyID);
        }

    }
    public class GetAllReleaseProdOrderLineAppQueryHandler : IRequestHandler<GetAllReleaseProdOrderLineAppQuery, List<ReleaseProdOrderLine>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappponumberQueryRepository;
        public GetAllReleaseProdOrderLineAppQueryHandler(IProductionActivityAppQueryRepository productionactivityappponumberQueryRepository)
        {
            _productionactivityappponumberQueryRepository = productionactivityappponumberQueryRepository;
        }
        public async Task<List<ReleaseProdOrderLine>> Handle(GetAllReleaseProdOrderLineAppQuery request, CancellationToken cancellationToken)
        {
            return (List<ReleaseProdOrderLine>)await _productionactivityappponumberQueryRepository.GetAllReleaseProdOrderLineAsyncPO(request.companyID);
        }

    }
    public class GetSupportingDocumentsHandler : IRequestHandler<GetSupportingDocuments, List<DocumentsModel>>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public GetSupportingDocumentsHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<List<DocumentsModel>> Handle(GetSupportingDocuments request, CancellationToken cancellationToken)
        {
            return (List<DocumentsModel>)await _productionactivityappQueryRepository.GetSupportingDocumentsAsync(request.ProductionActivityPlanningAppLineID,request.Type);
        }

    }
    public class InserProductionActivityEmailHandler : IRequestHandler<InserProductionActivityEmail, ActivityEmailTopicsModel>
    {
        private readonly IProductionActivityAppQueryRepository _productionactivityappQueryRepository;
        public InserProductionActivityEmailHandler(IProductionActivityAppQueryRepository productionactivityappQueryRepository)
        {
            _productionactivityappQueryRepository = productionactivityappQueryRepository;
        }
        public async Task<ActivityEmailTopicsModel> Handle(InserProductionActivityEmail request, CancellationToken cancellationToken)
        {
            return await _productionactivityappQueryRepository.InserProductionActivityEmail(request.ActivityEmailTopicsModel);
        }

    }
    public class CreateProductionRoutineAppQueryHandler : IRequestHandler<CreateProductionActivityRoutineAppCommand, long>
    {
        private readonly IProductionActivityAppQueryRepository _PPAppLineListQueryRepository;
        public CreateProductionRoutineAppQueryHandler(IProductionActivityAppQueryRepository PPAppLineListQueryRepository, IQueryRepository<ProductActivityAppModel> queryRepository)
        {
            _PPAppLineListQueryRepository = PPAppLineListQueryRepository;
        }

        public async Task<long> Handle(CreateProductionActivityRoutineAppCommand request, CancellationToken cancellationToken)
        {
            var newlist = await _PPAppLineListQueryRepository.InsertProductionRoutine(request);
            return newlist;

        }
    }
   
}

