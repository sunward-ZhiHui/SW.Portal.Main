using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class LoginSessionHistory
    {
        public long LoginSessionHistoryId { get; set; }
        public Guid? SessionId { get; set; }
        public long? UserId { get; set; }
        public string? LoginType { get; set; }
        public DateTime? LoginTime { get; set; }
        public string? LogoutType { get; set; }
        public DateTime? LogoutTime { get; set; }
        public DateTime? LastActivityTime { get; set; }
        public bool? IsActive { get; set; }=false;
        public string? IpAddress { get; set; }
        public string? UserAgent {  get; set; }
        [NotMapped]
        public string? UserName { get;set; }
        [NotMapped]
        public string? DepartmentName { get; set; }
        [NotMapped]
        public string? DesignationName { get; set; }
        [NotMapped]
        public string? EmployeeName { get; set; }
    }
}
