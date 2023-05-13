using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeICTInformation
    {
        [Key]
        public long EmployeeIctinformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? SoftwareId { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public long? RoleId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsPortal { get; set; }
    }
}
