using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class IpirAppModel
    {
        
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
        public long? ReportingPersonal { get; set; }
        public long? DetectedBy { get; set; }
        public string? AddedBy { get; set; }
        public string? ReportingPersonalName { get; set; }
        public string? DetectedByName { get; set; }
        public string? MachineName { get; set; }
        public string? RefNo { get; set; }
        public string? LocationName { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? CompanyName { get; set; }
        public string? ProfileName { get; set; }
        public long? ActivityStatusId { get; set; }
       
        public IEnumerable<long?> DepartmentIds { get; set; } = new List<long?>();
        public IEnumerable<long?> ActivityIssueRelateIds { get; set; } = new List<long?>();

    }
}
