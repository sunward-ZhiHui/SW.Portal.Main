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
            return await _queryRepository.GetNavItemServicesList(request.CompanyId, request.UserId);

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
        private readonly INavItemsQueryRepository _queryRepository;

        public DeleteItemBatchInfoHandler(INavItemsQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<ItemBatchInfo> Handle(DeleteItemBatchInfo request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteItemBatchInfo(request.ItemBatchInfo);
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
    public class GetNavprodOrderLineListAllQueryHandler : IRequestHandler<GetNavprodOrderLineListAllQuery, List<NavprodOrderLine>>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetNavprodOrderLineListAllQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<NavprodOrderLine>> Handle(GetNavprodOrderLineListAllQuery request, CancellationToken cancellationToken)
        {
            return (List<NavprodOrderLine>)await _queryRepository.GetNavprodOrderLineListAsync();

        }
    }
    public class DeleteNavprodOrderLineHandler : IRequestHandler<DeleteNavprodOrderLine, NavprodOrderLine>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public DeleteNavprodOrderLineHandler(INavItemsQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<NavprodOrderLine> Handle(DeleteNavprodOrderLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteNavprodOrderLine(request.NavprodOrderLine);
        }
    }
    public class InsertOrUpdateNavprodOrderLineHandler : IRequestHandler<InsertOrUpdateNavprodOrderLine, NavprodOrderLine>
    {


        private readonly INavItemsQueryRepository _queryRepository;
        public InsertOrUpdateNavprodOrderLineHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavprodOrderLine> Handle(InsertOrUpdateNavprodOrderLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateNavprodOrderLine(request);

        }
    }


    public class GetFinishedProdOrderLineAllQueryHandler : IRequestHandler<GetFinishedProdOrderLineAllQuery, List<FinishedProdOrderLine>>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetFinishedProdOrderLineAllQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<FinishedProdOrderLine>> Handle(GetFinishedProdOrderLineAllQuery request, CancellationToken cancellationToken)
        {
            return (List<FinishedProdOrderLine>)await _queryRepository.GeFinishedProdOrderLineListAsync();

        }
    }
    public class DeleteFinishedProdOrderLineHandler : IRequestHandler<DeleteFinishedProdOrderLine, FinishedProdOrderLine>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public DeleteFinishedProdOrderLineHandler(INavItemsQueryRepository QueryRepository)
        {
            _queryRepository = QueryRepository;
        }

        public async Task<FinishedProdOrderLine> Handle(DeleteFinishedProdOrderLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteFinishedProdOrderLine(request.FinishedProdOrderLine);
        }
    }
    public class InsertOrUpdateFinishedProdOrderLineHandler : IRequestHandler<InsertOrUpdateFinishedProdOrderLine, FinishedProdOrderLine>
    {


        private readonly INavItemsQueryRepository _queryRepository;
        public InsertOrUpdateFinishedProdOrderLineHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<FinishedProdOrderLine> Handle(InsertOrUpdateFinishedProdOrderLine request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateFinishedProdOrderLine(request);

        }
    }
    public class GetFinishedProdOrderLineOptStatusQueryHandler : IRequestHandler<GetFinishedProdOrderLineOptStatusQuery, List<FinishedProdOrderLineOptStatus>>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetFinishedProdOrderLineOptStatusQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<FinishedProdOrderLineOptStatus>> Handle(GetFinishedProdOrderLineOptStatusQuery request, CancellationToken cancellationToken)
        {
            return (List<FinishedProdOrderLineOptStatus>)await _queryRepository.GetFinishedProdOrderLineOptStatus();

        }
    }
    public class GetRawMatItemListByTypeListQueryHandler : IRequestHandler<GetRawMatItemListByTypeList, List<RawMatItemList>>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetRawMatItemListByTypeListQueryHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<RawMatItemList>> Handle(GetRawMatItemListByTypeList request, CancellationToken cancellationToken)
        {
            return (List<RawMatItemList>)await _queryRepository.GetRawMatItemListByTypeList(request.Type, request.CompanyId);

        }
    }


    public class InsertOrUpdateNavVendorHandler : IRequestHandler<InsertOrUpdateNavVendor, SoCustomer>
    {


        private readonly INavItemsQueryRepository _queryRepository;
        public InsertOrUpdateNavVendorHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<SoCustomer> Handle(InsertOrUpdateNavVendor request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetNavVendorList(request.CompanyId);

        }
    }
    public class InsertOrUpdateNavCustomerHandler : IRequestHandler<InsertOrUpdateNavCustomer, Navcustomer>
    {


        private readonly INavItemsQueryRepository _queryRepository;
        public InsertOrUpdateNavCustomerHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<Navcustomer> Handle(InsertOrUpdateNavCustomer request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetNavcustomerList(request.CompanyId);

        }
    }
    public class GetRawMatPurchListHandler : IRequestHandler<GetRawMatPurchList, RawMatPurch>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetRawMatPurchListHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<RawMatPurch> Handle(GetRawMatPurchList request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetRawMatPurchList(request.CompanyId);

        }
    }
    public class GetReleaseProdOrderLineListHandler : IRequestHandler<GetReleaseProdOrderLineList, ReleaseProdOrderLine>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetReleaseProdOrderLineListHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<ReleaseProdOrderLine> Handle(GetReleaseProdOrderLineList request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetReleaseProdOrderLineList(request.CompanyId);

        }
    }
    public class GetAllProdOrderLineListHandler : IRequestHandler<GetAllProdOrderLineList, AllProdOrderLine>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public GetAllProdOrderLineListHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<AllProdOrderLine> Handle(GetAllProdOrderLineList request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetAllProdOrderLineList(request.CompanyId);

        }
    }
    public class GetByGenericCodesHandler : IRequestHandler<GetByGenericCodes, List<GenericCodes>>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetByGenericCodesHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<GenericCodes>> Handle(GetByGenericCodes request, CancellationToken cancellationToken)
        {
            return (List<GenericCodes>)await _queryRepository.GetByGenericCodes();

        }
    }
    public class UpdateNavItemHandler : IRequestHandler<UpdateNavItem, View_NavItems>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public UpdateNavItemHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<View_NavItems> Handle(UpdateNavItem request, CancellationToken cancellationToken)
        {
            return await _queryRepository.UpdateNavItem(request.View_NavItems);

        }
    }


    public class GetNavPackingMethodLinesHandler : IRequestHandler<GetNavPackingMethodLines, List<NavPackingMethodModel>>
    {
        private readonly INavItemsQueryRepository _queryRepository;

        public GetNavPackingMethodLinesHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<List<NavPackingMethodModel>> Handle(GetNavPackingMethodLines request, CancellationToken cancellationToken)
        {
            return (List<NavPackingMethodModel>)await _queryRepository.GetNavPackingMethodLines(request.ItemId);

        }
    }

    public class DeleteNavPackingMethodHandler : IRequestHandler<DeleteNavPackingMethod, NavPackingMethodModel>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public DeleteNavPackingMethodHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavPackingMethodModel> Handle(DeleteNavPackingMethod request, CancellationToken cancellationToken)
        {
            return await _queryRepository.DeleteNavPackingMethod(request.NavPackingMethodModel);

        }
    }
    public class InsertOrUpdateNavPackingMethodHandler : IRequestHandler<InsertOrUpdateNavPackingMethod, NavPackingMethodModel>
    {
        private readonly INavItemsQueryRepository _queryRepository;
        public InsertOrUpdateNavPackingMethodHandler(INavItemsQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;

        }
        public async Task<NavPackingMethodModel> Handle(InsertOrUpdateNavPackingMethod request, CancellationToken cancellationToken)
        {
            return await _queryRepository.InsertOrUpdateNavPackingMethod(request);

        }
    }
}
