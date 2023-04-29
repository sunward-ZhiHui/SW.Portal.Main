using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewSubSection
    {
        public long SubSectionId { get; set; }
        [Required]
        public string SubSectionName { get; set; }
        public string? SubSectionCode { get; set; }
        public long? SectionId { get; set; }
        public long? DepartmentId { get; set; }
        public string? SectionName { get; set; }
        public string? SectionCode { get; set; }
        public string? Description { get; set; }
        public string? DepartmentName { get; set; }
        public string? DepartmentCode { get; set; }
        public int? HeadCount { get; set; }
        public long? DivisionId { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? ProfileCode { get; set; }
        public long? CompanyId { get; set; }
        public string? PlantCode { get; set; }
        public string? Company { get; set; }
        public string? Division { get; set; }
        public string? DivisionDescription { get; set; }
        public string? StatusCode { get; set; }
        public string? AddedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
