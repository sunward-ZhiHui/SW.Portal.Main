using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewDesignation
    {
        [Key]
        public long DesignationID { get; set; }
        public long? CompanyID { get; set; }
        [Required(ErrorMessage = "Designation Name is Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Level is Required")]
        public long? LevelID { get; set; }
        [Required(ErrorMessage = "Code is Required")]
        public string? Code { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Head Count is Required")]
        public int? HeadCount { get; set; }
        public string? CompanyName { get; set; }
        public int? CodeId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long? AddedByUserID { get; set; }
        [Required(ErrorMessage = "Status Code is Required")]
        public int? StatusCodeID { get; set; }
        public string? StatusCode { get; set; }
        public string? ModifiedBy { get; set; }
        public string? AddedBy { get; set; }
        public string? LevelName { get; set; }
        [Required(ErrorMessage = "SubSection is Required")]
        public long? SubSectionID { get; set; }
        public string? SubSectionName { get; set; }
        public string? SubSectionDescription { get; set; }
        public long? SectionID { get; set; }
        public string? SectionName { get; set; }
        public string? SectionDescription { get; set; }
        public long? DepartmentID { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentDescription { get; set; }
        public long? DivisionID { get; set; }
        public string? DivisionName { get; set; }
        public string? DivisionDescription { get; set; }
        public string? PlantCode { get; set; }
    }
}
