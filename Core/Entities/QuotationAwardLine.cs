using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class QuotationAwardLine
    {
        [Key]
        public long QuotationAwardLineId { get; set; }
        public long? QuotatonAwardId { get; set; }
        public decimal? QtyPerShipment { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        [NotMapped]
        [Range(1, Int64.MaxValue, ErrorMessage = "Only positive number allowed")]
        public int? QtyPerShipments { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }

    }
}
