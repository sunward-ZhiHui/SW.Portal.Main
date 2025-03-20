using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentViewer.Models
{
    public class DocumentsVersionTrace
    {
        public long? DocumentsVersionTraceId { get; set; }
        public string? Description { get; set; }
        public long? DocumentId { get; set; }
        public Guid? SessionId { get; set; }
        public long? UserId { get; set; }
        public DateTime? UpdateDateTime { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
    }
}
