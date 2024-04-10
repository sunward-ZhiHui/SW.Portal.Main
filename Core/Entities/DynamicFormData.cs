using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormData : BaseEntity
    {
        public long DynamicFormDataId { get; set; }
        public string? DynamicFormItem { get; set; }
        public bool? IsSendApproval { get; set; } = false;
        public long? DynamicFormId { get; set; }
        public Guid? FileProfileSessionID { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        [NotMapped]
        public AttributeHeaderListModel? AttributeHeader { get; set; }
        [NotMapped]
        public object? ObjectData { get; set; }
        [NotMapped]
        public string? DynamicFormCurrentItem { get; set; }
        [NotMapped]
        public string? Name { get; set; }
        [NotMapped]
        public string? ScreenID { get; set; }
        [NotMapped]
        public bool? IsApproval { get; set; }
        [NotMapped]
        public bool? IsApproved { get; set; }
        [NotMapped]
        public string? ApprovalStatus { get; set; }
        [NotMapped]
        public int? ApprovalStatusId { get; set; }
        [NotMapped]
        public string? RejectedUser { get; set; }
        [NotMapped]
        public DateTime? RejectedDate { get; set; }
        [NotMapped]

        public long? RejectedUserId { get; set; }
        [NotMapped]
        public string? PendingUser { get; set; }
        [NotMapped]
        public long? PendingUserId { get; set; }
        [NotMapped]
        public string? ApprovedUser { get; set; }
        [NotMapped]
        public DateTime? ApprovedDate { get; set; }
        [NotMapped]
        public long? ApprovedUserId { get; set; }
        [NotMapped]
        public List<DynamicFormApproved> DynamicFormApproved { get; set; }
        [NotMapped]
        public string? StatusName { get; set; }
        [NotMapped]
        public long? FileProfileTypeId { get; set; }
        [NotMapped]
        public string? FileProfileTypeName { get; set; }
        [NotMapped]
        public int? isDocuments { get; set; } = 0;
        [NotMapped]
        public int? IsFileprofileTypeDocument { get; set; } = 0;
        [NotMapped]
        public string? CurrentUserName { get; set; }
        [NotMapped]
        public long? CurrentUserId { get; set; }
        [NotMapped]
        public DynamicFormProfile DynamicFormProfile { get; set; } = new DynamicFormProfile();
        public long? DynamicFormDataGridId { get; set; }
        public bool? IsDynamicFormDataGrid { get; set; } = false;
        [NotMapped]
        public string? BackUrl { get; set; }
        public Guid? EmailTopicSessionId { get; set; }
        public bool? IsDraft { get; set; }
        public string? DynamicFormName { get; set; }
        public Guid? DynamicFormSessionID {  get; set; }
    }
    public class DynamicFormProfile
    {
        [Required(ErrorMessage = "Company is Required")]
        public long? PlantId { get; set; } = 0;
        [Required(ErrorMessage = "Department is Required")]
        public long? DepartmentId { get; set; } = 0;
        [Required(ErrorMessage = "Section is Required")]
        public long? SectionId { get; set; } = 0;
        [Required(ErrorMessage = "SubSection is Required")]
        public long? SubSectionId { get; set; } = 0;
        [Required(ErrorMessage = "Division is Required")]
        public long? DivisionId { get; set; } = 0;
        public long? ProfileId { get; set; } = 0;
        public long? UserId { get; set; }
        public string? ProfileNo { get; set; }
    }
    public class DynamicFormDataWrokFlow
    {
        public long DynamicFormDataId { get; set; }
        public long? DynamicFormId { get; set; }
        public Guid? SessionID { get; set; }
        public Guid? DynamicFormSessionID { get; set; }
        public long? ProfileId { get; set; }
        public string? ProfileNo { get; set; }
        public string? Name { get; set; }
        public string? ScreenID { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public List<DynamicFormWorkFlowSection> DynamicFormWorkFlowSections = new List<DynamicFormWorkFlowSection>();
        public string? SectionName { get; set; }
        public List<long?> DynamicFormSectionIds { get; set; }=new List<long?>();
        public string? UserIds { get; set; }
        public string? UserNames { get; set; }
        public string? StatusName { get; set; }
        public long? DynamicFormDataGridId { get; set; }
        public bool? IsDynamicFormDataGrid { get; set; } = false;
        public Guid? EmailTopicSessionId { get; set; }
        public bool? IsDraft { get; set; }
        [NotMapped]
        public int? IsFileprofileTypeDocument { get; set; } = 0;
        public long? FileProfileTypeId { get; set; }
    }
}
