using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_SoSalesOrder
    {
        public long SoSalesOrderId { get; set; }
        public int Index { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string SignOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public long? SoCustomerId { get; set; }
        public DateTime? PrioityDeliveryDate { get; set; }
        public string Remark { get; set; }
        public string ShipCode { get; set; }
        public string CustomerName { get; set; }
        public string AssignToRep { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string PostCode { get; set; }
        public string Channel { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public Guid? SessionId { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public int? CodeId { get; set; }
        public long? SoCustomerBillingAddressId { get; set; }
        public long? SoCustomerShipingAddressId { get; set; }
    }
}
