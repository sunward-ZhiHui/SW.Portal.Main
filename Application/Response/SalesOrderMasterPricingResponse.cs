using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class SalesOrderMasterPricingResponse
    {
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
        public string ErrorMessage { get; set; }
    }
}
