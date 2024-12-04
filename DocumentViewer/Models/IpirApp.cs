using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class IpirApp
    {
        [Key]
        public long IpirAppId { get; set; }
        public long? CompanyID { get; set; }
        public string? ProdOrderNo { get; set; }
        public int? StatusCodeID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionID { get; set; }
        public string? Comment { get; set; }
        public long? LocationID { get; set; }
        public long? NavprodOrderLineID { get; set; }
        public string? FixedAssetNo { get; set; }
        public long? ReportingPersonal { get; set; }
        public string? RefNo { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? StatusType { get; set; }
        public long? DetectedBy { get; set; }
        public long? ActivityStatusID { get; set; }
        public string? MachineName { get; set; }
        public string? SubjectName { get; set; }
        public string? Type { get; set; }
        public string? FPDD { get; set; }
        public string? ProcessDD { get; set; }
        public string? RawMaterialDD { get; set; }
        public string? PackingMaterialDD { get; set; }
        public string? FixedAsset { get; set; }
        
    }
}
