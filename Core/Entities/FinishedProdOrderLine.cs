using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FinishedProdOrderLine
    {
        public long FinishedProdOrderLineId { get; set; }
        public string? Status { get; set; }
        public string? ProdOrderNo { get; set; }
        public int? OrderLineNo { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description2 { get; set; }
        public string? ReplanRefNo { get; set; }
        public DateTime? StartingDate { get; set; }
        public string? BatchNo { get; set; }
        public DateTime? ManufacturingDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public long? CompanyId { get; set; }
        public string? OptStatus { get; set; }
        public long? ItemId { get; set; }
    }
}
