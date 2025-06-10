using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class SalesOrderModel : BaseModel
    {
        public long SalesOrderId { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileName { get; set; }
        public string? DocumentNo { get; set; }
        public DateTime? DateOfOrder { get; set; }
        public string? Ponumber { get; set; }
        public long? PurchaseOrderIssueId { get; set; }
        public string? PurchaseOrderIssue { get; set; }
        public long? CustomerId { get; set; }
        public string? Customer { get; set; }
        public DateTime? RequestShipmentDate { get; set; }
        public long? ShipToCodeId { get; set; }
        public string? ShipToCode { get; set; }
        public DateTime? VanDeliveryDate { get; set; }
        public string? ShipingCodeType { get; set; }
        public long? SobyCustomersSalesAddressId { get; set; }
    }
}
