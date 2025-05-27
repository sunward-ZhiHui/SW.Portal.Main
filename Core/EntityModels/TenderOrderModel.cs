using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class TenderOrderModel
    {
        public long SimualtionAddhocID { get; set; }
        public string? DocumantType { get; set; }
        public string? SelltoCustomerNo { get; set; }
        public string? CustomerName { get; set; }
        public string? Categories { get; set; }
        public string? DocumentNo { get; set; }
        public string? ExternalDocNo { get; set; }
        public long? ItemId { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? Description1 { get; set; }
        public decimal? OutstandingQty { get; set; }
        public DateTime? PromisedDate { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string? UOMCode { get; set; }
        public string? Company { get; set; }
        public byte[]? ByteData { get; set; }
        public long? UserId { get; set; }
        public long? CompanyId { get; set; }
    }

}
