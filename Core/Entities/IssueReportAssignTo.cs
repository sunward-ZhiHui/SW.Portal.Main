using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class IssueReportAssignTo
    {
        [Key]
        public long IPIRAssignToID { get; set; }
        public long IPIRId { get; set; }
        public long AssignToId { get; set; }
        public long ReportinginformationID { get; set; }
    }
}
