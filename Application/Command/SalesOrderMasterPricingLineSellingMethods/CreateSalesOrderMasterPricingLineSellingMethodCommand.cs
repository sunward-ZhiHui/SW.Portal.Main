using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SalesOrderMasterPricingLineSellingMethods
{
    public class CreateSalesOrderMasterPricingLineSellingMethodCommand : IRequest<SalesOrderMasterPricingLineSellingMethodResponse>
    {
        public long SalesOrderMasterPricingLineSellingMethodId { get; set; }
        public long? SalesOrderMasterPricingLineId { get; set; }
        public decimal? TierQty { get; set; }
        public decimal? TierPrice { get; set; }
        public decimal? BounsPrice { get; set; }
        public decimal? BounsQty { get; set; }
        public decimal? BounsFocQty { get; set; }
        public string? TierQtyType { get; set; }
        public CreateSalesOrderMasterPricingLineSellingMethodCommand()
        {

        }
    }
}
 