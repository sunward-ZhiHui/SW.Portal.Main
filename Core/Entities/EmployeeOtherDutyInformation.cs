using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeOtherDutyInformation
    {
        [Key]
        public long EmployeeOtherDutyInformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? DesignationTypeId { get; set; }
        public long? SubSectionTid { get; set; }
        public long? DesignationId { get; set; }
        public long? DutyTypeId { get; set; }
        public int? HeadCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
