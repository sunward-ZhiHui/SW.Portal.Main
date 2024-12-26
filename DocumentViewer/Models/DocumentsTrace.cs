using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class DocumentsTrace
    {
        [Key]
        public long DocumentsTraceId { get; set; }
        public long? DocumentId { get; set; }
        public Guid? SessionId { get; set; }
        public long? PrevUserId { get; set; }
        public long? CurrentUserId { get; set; }
        public DateTime? UpdateDateTime { get; set; }
    }
}
