namespace DocumentViewer.Models
{
    public class DocumentDmsShare
    {
        public long DocumentDmsShareId { get; set; }
        public long? DocumentId { get; set; }
        public Guid? DocSessionId { get; set; }
        public bool? IsExpiry { get; set; } 
        public DateTime? ExpiryDate { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserID { get; set; }
        public int? StatusCodeID { get; set; }
    }
}
