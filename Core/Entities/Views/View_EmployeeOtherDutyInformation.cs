using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_EmployeeOtherDutyInformation
    {
        public long EmployeeOtherDutyInformationId { get; set; }
        public long? EmployeeId { get; set; }
        public long? CompanyId { get; set; }
        public long? DivisionId { get; set; }
        public long? LevelId { get; set; }
        public long? SectionId { get; set; }
        public long? DepartmentId { get; set; }
        public long? SubSectionId { get; set; }
        [Required(ErrorMessage = "Designation is Required")]
        public long? DesignationId { get; set; }
        [Required(ErrorMessage = "Duty Type is Required")]
        public long? DutyTypeId { get; set; }
        public long? DesignationTypeId { get; set; }
        public int? HeadCount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public string SectionName { get; set; }
        public string SubSectionName { get; set; }
        public string DivisionName { get; set; }
        public string CompanyName { get; set; }
        public string LevelName { get; set; }
        public string DutyType { get; set; }
        public string StatusCode { get; set; }
    }
}
