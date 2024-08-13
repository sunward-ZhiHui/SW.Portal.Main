using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class IPIRReportingInformationModel
    {

        public long ReportinginformationID { get; set; }
        public long IpirAppID { get; set; }
        public string? IssueDescription { get; set; }
        public bool ReportBy { get; set; }
        public long? IssueRelatedTo { get; set; }
        public IEnumerable<long> AssignToIds { get; set; } = new List<long>();
       // public string? Message { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public string? FileName { get; set; }
      
        public string? ProfileNo { get; set; }
        public string? IssueRelatedName { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
    }
}
