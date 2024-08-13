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
        public IEnumerable<long> AssignToIds { get; set; } = new List<long>();
        [NotMapped]
        public string? Message { get; set; }
        [NotMapped]
        public long? DocumentId { get; set; }
        [NotMapped]
        public long? FileProfileTypeId { get; set; }
        [NotMapped]
        public long? DocumentID { get; set; }
        public string? FilePath { get; set; }
        [NotMapped]
        public Guid? UniqueSessionId { get; set; }
        [NotMapped]
        public bool? IsNewPath { get; set; }
        [NotMapped]
        public long? DocumentParentId { get; set; }
        [NotMapped]
        public string? FileName { get; set; }
        [NotMapped]
        public string? ContentType { get; set; }
        [NotMapped]
        public bool? IsLocked { get; set; }
        [NotMapped]
        public long? LockedByUserId { get; set; }
        [NotMapped]
        public string? ModifiedByUser { get; set; }
        [NotMapped]
        public string? LockedByUser { get; set; }
        [NotMapped]
        public string? ProfileNo { get; set; }
        [NotMapped]
        public string?  IssueRelatedName { get; set; }
    }
}
