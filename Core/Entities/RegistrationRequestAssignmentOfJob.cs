using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities.CustomValidations;

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
        public string? Type { get; set; }
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
        public long? RegistrationRequestDepartmentId { get; set; }
        public Guid? DepartmentUniqueSessionId { get; set; }
        public bool? IsEmailCreateDone { get; set; } = false;
        public Guid? EmailCreateSessionId { get; set; }
        public DateTime? CommittmentDate { get; set; }
        public string? CommitmentInformation { get; set; }
        public long? ActionByDeptId { get; set; }
        public DateTime? CommitmentTime { get; set; }
        public string? CommitmentStatus { get; set; }

        public DateTime? DateOfQueries { get; set; }
        public string? Requirement { get; set; }
        public string? Assignment { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DepartmentDueDate { get; set; }
    }
    public class RegistrationRequestDepartment
    {
        public long RegistrationRequestDepartmentId { get; set; }
        public long? RegistrationRequestId { get; set; }
        public long? DepartmentId { get; set; }
        public Guid? DepartmentUniqueSessionId { get; set; }
    }
    public class RegistrationRequestDepartmentEmailCreate
    {
        public string? From { get; set; }
        public long? FromId { get; set; }
        [Required(ErrorMessage = "Subject is Required")]
        public string? MainSubjectName { get; set; }
        [Required(ErrorMessage = "To is Required")]
        [RegistrationRequestCustomValidation]
        public IEnumerable<long>? ToIds { get; set; } = new List<long>();
        public IEnumerable<long>? CCIds { get; set; } = new List<long>();
        public List<RegistrationRequestAssignmentOfJob> RegistrationRequestAssignmentOfJobs { get; set; } = new List<RegistrationRequestAssignmentOfJob>();
        public IEnumerable<long>? ToUserGroupIds { get; set; } = null;
        public IEnumerable<long>? CCUserGroupIds { get; set; }
        public Guid? EmailCreateSessionId { get; set; }
        public string? BackUrl { get; set; }
    }
}
