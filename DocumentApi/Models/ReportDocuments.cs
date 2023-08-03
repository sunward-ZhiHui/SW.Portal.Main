using System.ComponentModel.DataAnnotations;

namespace DocumentApi.Models
{
    public class ReportDocuments
    {
        [Key]
        public long ReportDocumentID { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public int FileSize { get; set; }
        public Guid? SessionId { get; set; }
        public string FilePath { get; set; }
        public bool? IsLatest { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
