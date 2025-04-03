using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class CompanyListingModel : BaseModel
    {
        public long? CompanyListingID { get; set; }
        public long? ProfileID { get; set; }
        public string? ProfileReferenceNo { get; set; }
        public string? CompanyListingName { get; set; }
        public long? ItemClassificationMasterId { get; set; }
        public string? No { get; set; }
        public string? ProfileName { get; set; }
        public List<long?> CompanyTypeIds { get; set; } = new List<long?>();
        public long? CustomerCodeId { get; set; }
        public string? CustomerCode { get; set; }
        public string? DistributionSalesCustomerName { get; set; }
        public List<long?> CustomerCodeIds { get; set; } = new List<long?>();
        public string? BlanketName { get; set; }
        public long? BuyingThroughId { get; set; }
        public string? BuyingThrough { get; set; }
        public bool? IsNonTransaction { get; set; }
        public long? LinkNonTransactionCompanyId { get; set; }

    }
}
