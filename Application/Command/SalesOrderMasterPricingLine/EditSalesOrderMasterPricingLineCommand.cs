using Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SalesOrderMasterPricingLine
{
    public class EditSalesOrderMasterPricingLineCommand : IRequest<SalesOrderMasterPricingLineResponse>
    {
        [Key]
        public long SalesOrderMasterPricingLineId { get; set; }
        public long? SalesOrderMasterPricingId { get; set; }
        public long? ItemId { get; set; }
        public long? SellingMethodId { get; set; }
        public decimal? SellingPrice { get; set; }
        public decimal? SmAllowPriceByPercent { get; set; }
        public decimal? SmAllowPriceBy { get; set; }
        public long? SmAllowPriceId { get; set; }
        public decimal? NewAllowPrice { get; set; }
        public bool? IsSalesManagerApprovePrice { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public EditSalesOrderMasterPricingLineCommand()
        {
        }

    }
}