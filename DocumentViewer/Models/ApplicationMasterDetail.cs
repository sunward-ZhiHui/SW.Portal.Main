namespace DocumentViewer.Models
{
    public class ApplicationMasterDetail
    {
        public long ApplicationMasterDetailId { get; set; }
        public long ApplicationMasterId { get; set; }
        public string? Value { get; set; }
        public string Description { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? ProfileId { get; set; }
        public long? FileProfileTypeId { get; set; }
    }
}
