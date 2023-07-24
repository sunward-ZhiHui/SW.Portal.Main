using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class SalesOrderMasterPricingLineSellingMethodResponse
    {
        public long SalesOrderMasterPricingLineSellingMethodId { get; set; }
        public long? SalesOrderMasterPricingLineId { get; set; }
        public decimal? TierFromQty { get; set; }
        public decimal? TierToQty { get; set; }
        public decimal? TierPrice { get; set; }
        public decimal? BounsPrice { get; set; }
        public decimal? BounsQty { get; set; }
        public decimal? BounsFocQty { get; set; }
    }
}
