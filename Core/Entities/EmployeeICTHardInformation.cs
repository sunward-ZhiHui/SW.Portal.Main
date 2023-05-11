using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeICTHardInformation
    {
        [Key]
        public long EmployeeIctHardinformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? HardwareId { get; set; }
        public string LoginId { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Instruction { get; set; }
        public long? CompanyId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
