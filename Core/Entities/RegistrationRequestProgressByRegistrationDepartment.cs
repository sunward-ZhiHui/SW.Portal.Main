using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RegistrationRequestProgressByRegistrationDepartment
    {
        [Key]
        public long RegistrationRequestProgressByRegistrationDepartmentId { get; set; }
        public long? RegistrationRequestId { get; set; }
        public DateTime? ExpectedSubmissionDate { get; set; }
        public DateTime? ExpectedApprovalDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CCCloseDate { get; set; }
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
    }
    public class RegistrationRequestComittmentLetter
    {
        [Key]
        public long RegistrationRequestComittmentLetterId { get; set; }
        public long? RegistrationRequestProgressByRegistrationDepartmentId { get; set; }
        public DateTime? CommittmentDate { get; set; }
        public string? CommitmentInformation { get; set; }
        public long? ActionByDeptId { get; set; }
        public string? DepartmentName { get; set; }
        public DateTime? CommitmentTime { get; set; }
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
    }
    public class RegistrationRequestQueries
    {
        public long RegistrationRequestQueriesId { get; set; }
        public long? RegistrationRequestProgressByRegistrationDepartmentId { get; set; }
        public DateTime? DateOfQueries { get; set; }
        public string? Requirement { get; set; }
        public string? Assignment { get; set; }
        public DateTime? DueDate { get; set; }
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
    }
}
