using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RegistrationRequest
    {
        [Key]
        public long RegistrationRequestId { get; set; }
        [Required(ErrorMessage = "Registration Country is Required")]
        public long? RegistrationCountryId { get; set; }
        public string? CCNo { get; set; }
        public Guid? SessionId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? StatusCodeId { get; set; }
        public long? ProductSpecificationDynamicFormId { get; set; }
        public long? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? ExpectedSubmissionDate { get; set; }
        public DateTime? SubmissionDate { get; set; }
        public string? SubmissionNo { get; set; }
        public string? PurposeOfRegistration { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? RegistrationCountry { get; set; }
        [NotMapped]
        public string? StatusCode { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public List<long?> VariationNoIds { get; set; } = new List<long?>();
        public string? ProductSpecificationDynamicForm { get; set; }
        public string? VariationNo { get; set; }
        public object? ObjectData { get; set; }
    }
    public class RegistrationRequestVariation
    {
        [Key]
        public long RegistrationRequestVariationId { get; set; }
        public long? RegistrationRequestId { get; set; }
        public long? DynamicFormDataId { get; set; }
    }
    public class RegistrationRequestVariationForm
    {
        public long RegistrationRequestVariationFormId { get; set; }
        public long? RegistrationRequestVariationId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? RegDynamicFormDataID { get; set; }
        public string? Description { get; set; }
        public string? RegDynamicFormDataNameId { get; set; }
    }
    public class RegistrationRequestDueDateAssignment
    {
        [Key]
        public long RegistrationRequestDueDateAssignmentId { get; set; }
        public long? RegistrationRequestId { get; set; }
        [Required(ErrorMessage = "Department is Required")]
        public long? DepartmentId { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Description { get; set; }
        public string? DepartmentName { get; set; }
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
