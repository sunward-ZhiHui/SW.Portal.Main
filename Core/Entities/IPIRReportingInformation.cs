using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class IPIRReportingInformation:BaseEntity
    {
        [Key]
        public long ReportinginformationID { get; set; }
        public long IpirAppID { get; set; }
        public  string? IssueDescription { get; set; }
        public bool ReportBy {  get; set; }
        public  long? IssueRelatedTo { get; set; }
        [NotMapped]
        public IEnumerable<long?> AssignToIds { get; set; } = new List<long?>();
        [NotMapped]
        public string? Message { get; set; }
    }
}
