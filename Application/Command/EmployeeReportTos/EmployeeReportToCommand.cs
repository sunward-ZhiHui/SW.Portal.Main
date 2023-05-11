using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.EmployeeReportTos
{
    public class EmployeeReportToCommand
    {
        public long EmployeeReportToId { get; set; }
        public long? EmployeeId { get; set; }
        public long? ReportToId { get; set; }
        public EmployeeReportToCommand()
        {

        }
    }
}
