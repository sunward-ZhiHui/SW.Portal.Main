using Application.Queries;
using Application.Queries.Base;
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
    public class GetAllNavItemsHandler : IRequestHandler<GetAllNavItemsQuery, List<View_NavItems>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_NavItems>> Handle(GetAllNavItemsQuery request, CancellationToken cancellationToken)
        {
            return (List<View_NavItems>)await _queryRepository.GetAsyncList();
        }
    }
    public class GetAllNavItemsByCompanyHandler : IRequestHandler<GetAllNavItemsByCompanyQuery, List<View_NavItems>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsByCompanyHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_NavItems>> Handle(GetAllNavItemsByCompanyQuery request, CancellationToken cancellationToken)
        {
            return (List<View_NavItems>)await _queryRepository.GetByCompanyAsyncList(request.CompanyId);
        }
    }
    public class GetAllNavItemsByCompanyBySerialNoHandler : IRequestHandler<GetAllNavItemsByCompanyByItemSerialNoQuery, List<View_NavItems>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsByCompanyBySerialNoHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_NavItems>> Handle(GetAllNavItemsByCompanyByItemSerialNoQuery request, CancellationToken cancellationToken)
        {
            return (List<View_NavItems>)await _queryRepository.GetByCompanyBySerailNoAsyncList(request.CompanyId);
        }
    }

    public class GetAllNavItemsItemSerialNoHandler : IRequestHandler<GetAllNavItemsItemSerialNoQuery, View_NavItems>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsItemSerialNoHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<View_NavItems> Handle(GetAllNavItemsItemSerialNoQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetByItemSerialNoAsync(request.ItemSerialNo);
        }
    }
    public class GetAllNavItemsItemSerialNoNotNullHandler : IRequestHandler<GetAllNavItemsItemSerialNoNotNullQuery, List<View_NavItems>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemsItemSerialNoNotNullHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_NavItems>> Handle(GetAllNavItemsItemSerialNoNotNullQuery request, CancellationToken cancellationToken)
        {
            return (List<View_NavItems>)await _queryRepository.GetByItemSerialNoNotNullAsync();
        }
    }
    public class GetAllNavItemBatchNoByItemIdHandler : IRequestHandler<GetAllNavItemBatchNoByItemIdQuery, List<ItemBatchInfo>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavItemBatchNoByItemIdHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ItemBatchInfo>> Handle(GetAllNavItemBatchNoByItemIdQuery request, CancellationToken cancellationToken)
        {
            return (List<ItemBatchInfo>)await _queryRepository.GetNavItemBatchNoByItemIdAsync(request.ItemId, request.CompanyId);
        }
    }
    public class GetAllNavProductionInformationHandler : IRequestHandler<GetNavProductionInformationQuery, List<NavProductionInformation>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavProductionInformationHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<NavProductionInformation>> Handle(GetNavProductionInformationQuery request, CancellationToken cancellationToken)
        {
            return (List<NavProductionInformation>)await _queryRepository.GetNavProductionInformation(request.ItemId);
        }
    }
    public class GetAllNavCrossReferenceHandler : IRequestHandler<GetNavCrossReferenceQuery, List<View_NavCrossReference>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllNavCrossReferenceHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<View_NavCrossReference>> Handle(GetNavCrossReferenceQuery request, CancellationToken cancellationToken)
        {
            return (List<View_NavCrossReference>)await _queryRepository.GetNavCrossReference(request.ItemId);
        }
    }
    public class GetSyncBatchQueryHandler : IRequestHandler<GetSyncBatchQuery, ItemBatchInfo>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetSyncBatchQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<ItemBatchInfo> Handle(GetSyncBatchQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetSyncBatchInfo(request.ItemNo, request.CompanyId, request.ItemId);
        }
    }
    public class NavCompanyItemBatchInfoQueryHandler : IRequestHandler<NavCompanyItemBatchInfoQuery, ItemBatchInfo>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public NavCompanyItemBatchInfoQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<ItemBatchInfo> Handle(NavCompanyItemBatchInfoQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetNavItemBatchInfo(request.CompanyId);

        }
    }
    public class NavCompanyItemQueryHandler : IRequestHandler<NavCompanyItemQuery, Navitems>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public NavCompanyItemQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<Navitems> Handle(NavCompanyItemQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetNavItemServicesList(request.CompanyId,request.UserId);

        }
    }
    public class GetFinishedProdOrderLineQueryHandler : IRequestHandler<GetFinishedProdOrderLineQuery, FinishedProdOrderLine>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetFinishedProdOrderLineQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<FinishedProdOrderLine> Handle(GetFinishedProdOrderLineQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetFinishedProdOrderLineList(request.CompanyId);

        }
    }
    public class GetItemBatchInfoAllQueryHandler : IRequestHandler<GetItemBatchInfoAllQuery, List<ItemBatchInfo>>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetItemBatchInfoAllQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }
        public async Task<List<ItemBatchInfo>> Handle(GetItemBatchInfoAllQuery request, CancellationToken cancellationToken)
        {
            return (List<ItemBatchInfo>)await _queryRepository.GetNavItemBatchNoByItemByAllAsync();
        }
    }
    public class InsertOrUpdateBatchInfoHandler : IRequestHandler<InsertOrUpdateBatchInfo, ItemBatchInfo>
    {


        private readonly INavItemsQueryRepository _queryRepository;
        public InsertOrUpdateBatchInfoHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ItemBatchInfo> Handle(InsertOrUpdateBatchInfo request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateBatchInfo(request);

        }
    }
    public class DeleteItemBatchInfoHandler : IRequestHandler<DeleteItemBatchInfo, ItemBatchInfo>
    {
        private readonly INavItemsQueryRepository _DynamicFormQueryRepository;

        public DeleteItemBatchInfoHandler(INavItemsQueryRepository QueryRepository)
        {
            _DynamicFormQueryRepository = QueryRepository;
        }

        public async Task<ItemBatchInfo> Handle(DeleteItemBatchInfo request, CancellationToken cancellationToken)
        {
            return await _DynamicFormQueryRepository.DeleteItemBatchInfo(request.ItemBatchInfo);
        }
    }
    public class GetNavprodOrderLineListQueryHandler : IRequestHandler<GetNavprodOrderLineListQuery, NavprodOrderLine>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetNavprodOrderLineListQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<NavprodOrderLine> Handle(GetNavprodOrderLineListQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetNavprodOrderLineList(request.CompanyId);

        }
    }
}
