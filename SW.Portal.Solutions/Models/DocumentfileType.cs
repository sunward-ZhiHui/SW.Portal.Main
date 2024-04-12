namespace SW.Portal.Solutions.Models
{
    public class DocumentfileType
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid SessionId { get; set; }
        public long ProfileID { get; set; }
        public long FileProfileTypeID { get; set; }
    }
}
