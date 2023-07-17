using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SalesOrderMasterPricing
    {
        [Key]
        public long SalesOrderMasterPricingId { get; set; }
        public long? CompanyId { get; set; }
        public long? SalesPricingForId { get; set; }
        public DateTime? PriceValidaityFrom { get; set; }
        public DateTime? PriceValidaityTo { get; set; }
        public long? ReasonForChangeId { get; set; }
        public string? MasterType { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        [NotMapped]
        public DateTime? Date { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
    }
}
