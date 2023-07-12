using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SoSalesOrder
    {
        [Key]
        public long SoSalesOrderId { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string SignOrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public long? SoCustomerId { get; set; }
        public DateTime? PrioityDeliveryDate { get; set; }
        public string Remark { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public Guid? SessionId { get; set; }
        public long? SoCustomerBillingAddressId { get; set; }
        public long? SoCustomerShipingAddressId { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
        [NotMapped]
        public string? ShipCode { get; set;}
        [NotMapped]
        public string? AssignToRep { get; set;}
        [NotMapped]
        public string? Address1 { get; set;}
        [NotMapped]
        public DateTime? Date { get; set; }
    }
}
