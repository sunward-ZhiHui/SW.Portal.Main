namespace SW.Portal.Solutions.Models
{
    public class DocumentsView
    {
        public long DocumentId { get; set; }
        public string? SubjectName { get; set; }
        public string? FileName { get; set; }
        public string? AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string? FilePath { get; set; }


    }
}
