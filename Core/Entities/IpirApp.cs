using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class IpirApp
    {
        [Key]
        public long IpirAppId { get; set; }
        public long? CompanyID { get; set; }
        public long? LocationID { get; set; }
        public int? StatusCodeID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionID { get; set; }
        public string? Comment { get; set; }
        public string? ProdOrderNo { get; set; }
        public long? NavprodOrderLineID { get; set; }
        public string? FixedAssetNo { get; set; }
        public string? ReportingPersonal { get; set; }
        public string? RefNo { get; set; }
    }
}
