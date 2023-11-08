using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductActivityCaseResponsResponsible
    {
        [Key]
        public long WikiResponsibilityId { get; set; }
        public long? EmployeeId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public long? ProductActivityCaseResponsDutyId { get; set; }
    }
}
