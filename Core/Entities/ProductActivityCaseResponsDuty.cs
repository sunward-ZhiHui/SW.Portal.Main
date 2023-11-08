using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductActivityCaseResponsDuty
    {
        [Key]
        public long ProductActivityCaseResponsDutyId { get; set; }
        public long? ProductActivityCaseResponsId { get; set; }
        public long? DutyNo { get; set; }
        public long? EmployeeId { get; set; }
        public long? CompanyId { get; set; }
        public long? DepartmantId { get; set; }
        public long? DesignationId { get; set; }
        public string Description { get; set; }
        public string DesignationNumber { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Type { get; set; }
    }
}
