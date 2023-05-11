using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeReportTo
    {
        [Key]
        public long EmployeeReportToId { get; set; }
        public long? EmployeeId { get; set; }
        public long? ReportToId { get; set; }
    }
}
