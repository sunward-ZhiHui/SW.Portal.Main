using Application.Queries.Base;
using Core.Entities;
using Core.Entities.Views;
using Core.EntityModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSoOrder : PagedRequest, IRequest<List<View_SoSalesOrder>>
    {
        public string SearchString { get; set; }
    }
    public class GetAllSoOrderBySession : PagedRequest, IRequest<View_SoSalesOrder>
    {
        public string SearchString { get; set; }
        public Guid? SesionId { get; set; }
        public GetAllSoOrderBySession(Guid? SessionId)
        {
            this.SesionId = SessionId;
        }
    }
    public class GetAllSoOrderByID : PagedRequest, IRequest<View_SoSalesOrder>
    {
        public string SearchString { get; set; }
        public long? Id { get; set; }
        public GetAllSoOrderByID(long? Id)
        {
            this.Id = Id;
        }
    }
    public class GetAllSalesOrderMasterPricingLineByItemByMethodQuery : View_SalesOrderMasterPricingLineByItem, IRequest<List<View_SalesOrderMasterPricingLineByItem>>
    {
        public long? CompanyId { get; set; }
        public DateTime? FromDate { get; set; }
        public long? SellingMethodId { get; set; }
        public long? ItemId { get; set; }
        public GetAllSalesOrderMasterPricingLineByItemByMethodQuery(long? CompanyId, DateTime? FromDate, long? SellingMethodId, long? ItemId)
        {
            this.CompanyId = CompanyId;
            this.FromDate = FromDate;
            this.SellingMethodId = SellingMethodId;
            this.ItemId = ItemId;
        }
    }
    public class GetAllSalesOrderMasterPricingLineByItemByQtyQuery : View_SalesOrderMasterPricingLineByItem, IRequest<SalesOrderMasterPricingFromSalesModel>
    {
        public long? CompanyId { get; set; }
        public DateTime? FromDate { get; set; }
        public long? SellingMethodId { get; set; }
        public long? ItemId { get; set; }
        public decimal? Qty { get; set; }
        public GetAllSalesOrderMasterPricingLineByItemByQtyQuery(long? CompanyId, DateTime? FromDate, long? SellingMethodId, long? ItemId, decimal? Qty)
        {
            this.CompanyId = CompanyId;
            this.FromDate = FromDate;
            this.SellingMethodId = SellingMethodId;
            this.ItemId = ItemId;
            this.Qty = Qty;
        }
    }

}
