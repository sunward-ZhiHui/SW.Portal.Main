namespace SW.Portal.Solutions.Models
{
    public class FileProfileTypeModel
    {
        public long UserID { get; set; }
        public long ProfileId { get; set; }
        public long? PlantId { get; set; } = 0;

       public int? StatusCodeID { get; set; }
       
        public long FileProfileTypeId { get; set; }
      
      
        public long? DepartmentId { get; set; } = 0;
      
        public long? SectionId { get; set; } = 0;
       
        public long? SubSectionId { get; set; } = 0;
       
        public long? DivisionId { get; set; } = 0;
        public string? CompanyCode { get; set; }
        public string? SectionName { get; set; }
        public string? SubSectionName { get; set; }
        public string? DepartmentName { get; set; }
        public Guid SessionId { get; set; }
        public long? addedByUserId { get; set; }
        public IFormFile File { get; set; }
       // public Guid? FileSessionId { get; set; }
    }
}
