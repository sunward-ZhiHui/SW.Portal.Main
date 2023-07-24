using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class SalesOrderMasterPricingFromSalesModel
    {
        public long? SalesOrderMasterPricingLineId { get; set; }
        public long? SalesOrderMasterPricingLineSellingMethodId { get; set; }
        public decimal? TierFromQty { get; set; }
        public decimal? TierToQty { get; set; }
        public decimal? TierPrice { get; set; }
        public decimal? BounsPrice { get; set; }
        public decimal? BounsQty { get; set; }
        public decimal? BounsFocQty { get; set; }
       
        public long? ItemId { get; set; }
        public long? SellingMethodId { get; set; }
        public long? CompanyId { get; set; }
        public decimal? SellingPrice { get; set; }
        public DateTime PriceValidaityFrom { get; set; }
        public DateTime PriceValidaityTo { get; set; }
        public string? SellingMethod { get; set; }
        public string? MasterType { get; set; }
    }
}
