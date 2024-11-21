using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllNavItemsQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
    }

    public class GetAllNavItemsItemSerialNoNotNullQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllNavItemsByCompanyQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
        public long? CompanyId { get; set; }
        public GetAllNavItemsByCompanyQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetAllNavItemsByCompanyByItemSerialNoQuery : PagedRequest, IRequest<List<View_NavItems>>
    {
        public string SearchString { get; set; }
        public long? CompanyId { get; set; }
        public GetAllNavItemsByCompanyByItemSerialNoQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetAllNavItemsItemSerialNoQuery : PagedRequest, IRequest<View_NavItems>
    {
        public string SearchString { get; set; }
        public string ItemSerialNo { get; set; }
        public GetAllNavItemsItemSerialNoQuery(string ItemSerialNo)
        {
            this.ItemSerialNo = ItemSerialNo;
        }
    }
    public class GetAllNavItemBatchNoByItemIdQuery : PagedRequest, IRequest<List<ItemBatchInfo>>
    {
        public string SearchString { get; set; }
        public long? ItemId { get; set; }
        public long? CompanyId { get; set; }
        public GetAllNavItemBatchNoByItemIdQuery(long? ItemId, long? CompanyId)
        {
            this.ItemId = ItemId;
            this.CompanyId = CompanyId;
        }
    }
    public class GetNavProductionInformationQuery : PagedRequest, IRequest<List<NavProductionInformation>>
    {
        public string SearchString { get; set; }
        public long? ItemId { get; set; }
        public GetNavProductionInformationQuery(long? ItemId)
        {
            this.ItemId = ItemId;
        }
    }
    public class GetNavCrossReferenceQuery : PagedRequest, IRequest<List<View_NavCrossReference>>
    {
        public string SearchString { get; set; }
        public long? ItemId { get; set; }
        public GetNavCrossReferenceQuery(long? ItemId)
        {
            this.ItemId = ItemId;
        }
    }
    public class GetSyncBatchQuery : PagedRequest, IRequest<ItemBatchInfo>
    {
        public string SearchString { get; set; }
        public string ItemNo { get; set; }
        public long? ItemId { get; set; }
        public long? CompanyId { get; set; }
        public GetSyncBatchQuery(string ItemNo, long? CompanyId, long? ItemId)
        {
            this.ItemNo = ItemNo;
            this.CompanyId = CompanyId;
            this.ItemId = ItemId;
        }
    }

    public class NavCompanyItemBatchInfoQuery : PagedRequest, IRequest<ItemBatchInfo>
    {
        public long? CompanyId { get; set; }
        public NavCompanyItemBatchInfoQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class NavCompanyItemQuery : PagedRequest, IRequest<Navitems>
    {
        public long? CompanyId { get; set; }
        public long? UserId { get; set; }
        public NavCompanyItemQuery(long? CompanyId, long? userId)
        {
            this.CompanyId = CompanyId;
            this.UserId = userId;
        }
    }
    public class GetFinishedProdOrderLineQuery : PagedRequest, IRequest<FinishedProdOrderLine>
    {
        public long? CompanyId { get; set; }
        public GetFinishedProdOrderLineQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetItemBatchInfoAllQuery : PagedRequest, IRequest<List<ItemBatchInfo>>
    {
        public string SearchString { get; set; }
    }
    public class InsertOrUpdateBatchInfo : ItemBatchInfo, IRequest<ItemBatchInfo>
    {

    }
    public class DeleteItemBatchInfo : ItemBatchInfo, IRequest<ItemBatchInfo>
    {
        public ItemBatchInfo ItemBatchInfo { get; set; }
        public DeleteItemBatchInfo(ItemBatchInfo itemBatchInfo)
        {
            this.ItemBatchInfo = itemBatchInfo;
        }
    }
    public class GetNavprodOrderLineListQuery : PagedRequest, IRequest<NavprodOrderLine>
    {
        public long? CompanyId { get; set; }
        public GetNavprodOrderLineListQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
    public class GetNavprodOrderLineListAllQuery : PagedRequest, IRequest<List<NavprodOrderLine>>
    {
        
    }
    public class DeleteNavprodOrderLine : NavprodOrderLine, IRequest<NavprodOrderLine>
    {
        public NavprodOrderLine NavprodOrderLine { get; set; }
        public DeleteNavprodOrderLine(NavprodOrderLine navprodOrderLine)
        {
            this.NavprodOrderLine = navprodOrderLine;
        }
    }
    public class InsertOrUpdateNavprodOrderLine : NavprodOrderLine, IRequest<NavprodOrderLine>
    {

    }

    public class GetFinishedProdOrderLineAllQuery : PagedRequest, IRequest<List<FinishedProdOrderLine>>
    {

    }
    public class DeleteFinishedProdOrderLine : NavprodOrderLine, IRequest<FinishedProdOrderLine>
    {
        public FinishedProdOrderLine FinishedProdOrderLine { get; set; }
        public DeleteFinishedProdOrderLine(FinishedProdOrderLine finishedProdOrderLine)
        {
            this.FinishedProdOrderLine = finishedProdOrderLine;
        }
    }
    public class InsertOrUpdateFinishedProdOrderLine : FinishedProdOrderLine, IRequest<FinishedProdOrderLine>
    {

    }
    public class GetFinishedProdOrderLineOptStatusQuery : PagedRequest, IRequest<List<FinishedProdOrderLineOptStatus>>
    {

    }
    public class GetRawMatItemListByTypeList : PagedRequest, IRequest<List<RawMatItemList>>
    {
        public string? Type { get; set; }
        public long? CompanyId { get; set; }
        public GetRawMatItemListByTypeList(string? type, long? companyId)
        {
            this.Type = type;
            this.CompanyId = companyId;
        }
    }
}
