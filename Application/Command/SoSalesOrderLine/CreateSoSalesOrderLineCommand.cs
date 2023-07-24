using Application.Response;
using Core.Entities.Views;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.SoSalesOrderLine
{
    public class CreateSoSalesOrderLineCommand : IRequest<SoSalesOrderLineResponse>
    {

        public long SoSalesOrderLineId { get; set; }
        public long? SoSalesOrderId { get; set; }
        public string ItemSerialNo { get; set; }
        public decimal? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ItemId { get; set; }
        public long? SellingMethodId { get; set; }
        public decimal? ManualPrice { get; set; }
        public decimal? OrderBounsQty { get; set; }
        public string PricingType { get; set; }
        public bool? IsManual { get; set; }
        public CreateSoSalesOrderLineCommand()
        {

        }
    }
}

