using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_SoSalesOrderLine
    {
        public long SoSalesOrderLineId { get; set; }
        public int Index { get; set; }
        public long? SoSalesOrderId { get; set; }
        public string ItemSerialNo { get; set; }
        public decimal? Qty { get; set; }
        public decimal? UnitPrice { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public int? CodeId { get; set; }
        public string No { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string BaseUnitofMeasure { get; set; }
        public long? ItemId { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public string? NavCompany { get; set; }
        public string? ShipCode { get; set; }
    }
}
