using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class ApplicationUser
    {
        [Key]
        public long UserId { get; set; }
        public string? UserCode { get; set; }
        public string? UserName { get; set; }
        public string? EmployeeNo { get; set; }
        public string? UserEmail { get; set; }
        public byte AuthenticationType { get; set; }
        public string? LoginId { get; set; }
        public string? LoginPassword { get; set; }
        public long? DepartmentId { get; set; }
        public bool IsPasswordChanged { get; set; }
        public int? StatusCodeId { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? EmployeId { get; set; }
        public Guid? SessionId { get; set; }
        public int? InvalidAttempts { get; set; }
        public bool? Locked { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
    }
}
