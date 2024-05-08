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
        public NavCompanyItemQuery(long? CompanyId)
        {
            this.CompanyId = CompanyId;
        }
    }
}
