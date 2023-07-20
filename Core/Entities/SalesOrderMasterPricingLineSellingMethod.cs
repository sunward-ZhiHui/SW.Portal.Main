using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SalesOrderMasterPricingLineSellingMethod
    {
        [Key]
        public long SalesOrderMasterPricingLineSellingMethodId { get; set; }
        public long? SalesOrderMasterPricingLineId { get; set; }
        [Required(ErrorMessage = "Tier Qty is Required")]
        public decimal? TierQty { get; set; }
        [Required(ErrorMessage = "Tier Price is Required")]
        public decimal? TierPrice { get; set; }
        public string? TierQtyType { get; set; }
        public decimal? BounsPrice { get; set; }
        [Required(ErrorMessage = "Bonus Qty Required")]
        public decimal? BounsQty { get; set; }
        [Required(ErrorMessage = "Bouns Foc Qty is Required")]
        public decimal? BounsFocQty { get; set; }
    }
}
