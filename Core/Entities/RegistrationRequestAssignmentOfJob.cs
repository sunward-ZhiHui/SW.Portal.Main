using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RegistrationRequestAssignmentOfJob
    {
        [Key]
        public long RegistrationRequestAssignmentOfJobId { get; set; }
        public long? RegistrationRequestId { get; set; }
        [Required(ErrorMessage = "Department is Required")]
        public long? DepartmentId { get; set; }
        public DateTime? TargetDate { get; set; }
        [Required(ErrorMessage = "No is Required")]
        public string? JobNo { get; set; }
        public string? DepartmentName { get; set; }
        public string? DetailInforamtionByGuideline { get; set; }
        public string? DetailRequirement { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        public long? DynamicFormDataId { get; set; }
        public string? BackUrl { get; set; }
        public Guid? EmailTopicSessionId { get; set; }
        public bool? IsDraft { get; set; }
        public string? SubjectName { get; set; }
        public string? Comment { get; set; }
    }
}
