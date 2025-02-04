using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class IpirApp
    {
        [Key]
        public long IpirAppId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyID { get; set; }
        public long? LocationID { get; set; }
        public int? StatusCodeID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionID { get; set; }
        public string? Comment { get; set; }
       // [Required(ErrorMessage = "Prod OrderNo is Required")]
        public string? ProdOrderNo { get; set; }
        public string? Type { get; set; }
        public string? SubjectName { get; set; }
        public long? NavprodOrderLineID { get; set; }
        public string? FixedAssetNo { get; set; }
        public long? ReportingPersonal { get; set; }
        public long? DetectedBy { get; set; }
      //  [Required(ErrorMessage = "Machine is Required")]
        public string? MachineName { get; set; }
        public string? RefNo { get; set; }
       // [Required(ErrorMessage = "Profile is Required")]
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? StatusType { get; set; }
        public string? FPDD { get; set; }
        public string? ProcessDD { get; set; }
        public string? RawMaterialDD { get; set; }
        public string? PackingMaterialDD { get; set; }
        public string? FixedAsset { get; set; }
        [NotMapped]
        public string? CompanyCode { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? LocationName { get; set; }
        [NotMapped]
        public string? ItemNo { get; set; }
        [NotMapped]
        public string? RefPlanNo { get; set; }
        [NotMapped]
        public string? Description { get; set; }
        [NotMapped]
        public string? Description1 { get; set; }
        [NotMapped]
        public string? BatchNo { get; set; }
        [NotMapped]
        public string? ProfileName { get; set; }
        [NotMapped]
        public int? IsDocuments { get; set; }
        public string? ReportingPersonalName { get; set; }
        public string? DetectedByName { get; set; }
        public long? DocumentParentId { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public bool? IsLocked { get; set; }
        public long? LockedByUserId { get; set; }
        public string? ModifiedByUser { get; set; }
        public string? LockedByUser { get; set; }
        public long? DocumentId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? DocumentID { get; set; }
        public string? FilePath { get; set; }
        public Guid? UniqueSessionId { get; set; }
        public bool? IsNewPath { get; set; }
        [Required (ErrorMessage = "Activity Status is Required")]
        public long? ActivityStatusId { get; set; }
        public List<IpirAppIssueDep> ActivityIssueRelates { get; set; } = new List<IpirAppIssueDep>();
        public IEnumerable<long?> ActivityIssueRelateIds { get; set; } = new List<long?>();
        public IEnumerable<long?> DepartmentIds { get; set; } = new List<long?>();
        public string? DocProfileNo { get; set; }
        [NotMapped]
        public string? ProdOrderNoDescription { get; set; }
        [NotMapped]
        public string? StatusName { get; set; }
    }
    public class IpirAppIssueDep
    {
        [Key]
        public long IpirAppIssueDepId { get; set; }
        public long? IpirAppID { get; set; }
        public long? DepartmentID { get; set; }
        public long? ActivityInfoIssueId { get; set; }
        public string? Type { get; set; }
        public long? DynamicFormDataId { get; set; }
        [NotMapped]
        public string? IssueRelateName { get; set; }
        [NotMapped]
        public Guid? DynamicFormDataSessionId { get; set; }
        [NotMapped]
        public Guid? DynamicFormSessionId { get; set; }
    }
}
