namespace DocumentViewer.Models
{
    public class DocumentViewers
    {
        public long DocumentViewersId { get; set; }
        public long? UserId { get; set; }
        public string? Description { get; set; }
        public DateTime? AddedDate { get; set; }
        public Guid? SessionId { get; set; }
        public bool? IsMobile { get; set; } = false;
    }
}
