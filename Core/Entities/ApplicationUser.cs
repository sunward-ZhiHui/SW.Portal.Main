using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationUser : BaseEntity
    {
        [Key]
        public long UserID { get; set; }
        [Required(ErrorMessage = "Please enter your login name.")]
        public string? LoginID { get; set; }
        public string? LoginPassword { get; set; }
        [Required(ErrorMessage = "Please enter your password.")]
        public string UserName { get; set; }    
        public string UserCode { get; set; }
        public int InvalidAttempts { get; set; }
        public bool Locked { get; set; }
        public DateTime? LastPasswordChanged { get; set; }
        public string EmployeeNo { get; set; }
        public string UserEmail { get; set; }
        public byte AuthenticationType { get; set; }
        public long? DepartmentId { get; set; }
        public bool IsPasswordChanged { get; set; }
        public DateTime? LastAccessDate { get; set; }
        public string? OutlookEmailID { get; set; }
        public string? OutlookPassword { get; set; }
        public long? EmployeId { get; set; }
        [NotMapped]
        public string? Status { get; set; }
        [NotMapped]
        public List<string>? ColumnsToUpdate { get; set; }
        [NotMapped]
        public string? DepartmentName { get; set; }
        [NotMapped]
        public string? ModifiedByUser { get; set; }
    }
}
