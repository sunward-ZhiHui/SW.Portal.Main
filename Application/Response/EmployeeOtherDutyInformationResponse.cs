using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class EmployeeOtherDutyInformationResponse
    {
        public long EmployeeOtherDutyInformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? CompanyId { get; set; }
        public long? DivisionId { get; set; }
        public long? LevelId { get; set; }
        public long? SectionId { get; set; }
        public long? DepartmentId { get; set; }
        public long? SubSectionId { get; set; }
        public long? DesignationId { get; set; }
        public long? DutyTypeId { get; set; }
        public long? DesignationTypeId { get; set; }
        public int? HeadCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string DivisionName { get; set; }
        public string CompanyName { get; set; }
        public string LevelName { get; set; }
        public string DutyType { get; set; }
    }
}
