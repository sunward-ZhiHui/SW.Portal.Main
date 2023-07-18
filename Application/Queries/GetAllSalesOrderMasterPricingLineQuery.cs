using Application.Queries.Base;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries
{
    public class GetAllSalesOrderMasterPricingLineQuery : View_SalesOrderMasterPricingLine, IRequest<List<View_SalesOrderMasterPricingLine>>
    {
        public long SalesOrderMasterPricingId { get; set; }
        public GetAllSalesOrderMasterPricingLineQuery(long id)
        {
            this.SalesOrderMasterPricingId = id;
        }
    }
    public class GetAllSalesOrderMasterPricingLineByItemQuery : View_SalesOrderMasterPricingLineByItem, IRequest<List<View_SalesOrderMasterPricingLineByItem>>
    {
        public long? CompanyId { get; set; }
        public DateTime? PriceValidaityFrom { get; set; }
        public DateTime? PriceValidaityTo { get; set; }
        public long? ItemId { get; set; }
        public GetAllSalesOrderMasterPricingLineByItemQuery(long? CompanyId, DateTime? PriceValidaityFrom, DateTime? PriceValidaityTo, long? ItemId)
        {
            this.CompanyId = CompanyId;
            this.PriceValidaityFrom = PriceValidaityFrom;
            this.PriceValidaityTo = PriceValidaityTo;
            this.ItemId = CompanyId;
        }
    }

}
