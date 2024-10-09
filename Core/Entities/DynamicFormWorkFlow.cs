using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormWorkFlow
    {
        [Key]
        public long DynamicFormWorkFlowId { get; set; }
        public long? DynamicFormId { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public long? UserId { get; set; }
        public bool? IsAllowDelegateUser { get; set; } = false;
        public long? UserGroupId { get; set; }
        public long? LevelId { get; set; }
        [Required(ErrorMessage = "Sequence No is required")]
        [DynamicFormWorkFlowSequenceNoCustomValidation]
        public int? SequenceNo { get; set; }
        [NotMapped]
        public string? UserGroup { get; set; }
        [NotMapped]
        public string? UserGroupDescription { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public string? LevelName { get; set; }
        [NotMapped]
        public string? NickName { get; set; }
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string? LastName { get; set; }
        [NotMapped]
        public string? DepartmentName { get; set; }
        [NotMapped]
        public string? DesignationName { get; set; }
        [NotMapped]
        public string? FullName { get; set; }
        public IEnumerable<long?> SelectUserIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectUserGroupIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectLevelMasterIDs { get; set; } = new List<long?>();
        [NotMapped]
        [DynamicFormWorkFlowSectionNameCustomValidation]
        public IEnumerable<long>? SelectDynamicFormSectionIDs { get; set; } = new List<long>();
        public List<long> DynamicFormSectionIDs { get; set; } = new List<long>();
        public List<long> DynamicFormSectionAllIDs { get; set; } = new List<long>();
        public List<long> DynamicFormSectionIdList { get; set; } = new List<long>();
        public string? Type { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormWorkFlowFormId { get; set; }
        public bool? IsNextFlow { get; set; } = false;
        public string? DynamicFormName { get; set; }
        public string? DynamicFormSectionName { get; set; }
    }
    public class DynamicFormWorkFlowSection
    {
        [Key]
        public long DynamicFormWorkFlowId { get; set; }
        public long? DynamicFormWorkFlowSectionId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public long? DynamicFormId { get; set; }
        public long? UserId { get; set; }
        [NotMapped]
        public long? UserGroupId { get; set; }
        [NotMapped]
        public long? LevelId { get; set; }
        public string? Type { get; set; }
        public int? IsWorkFlowDone { get; set; }
        public int? SequenceNo { get; set; }
        public string? SectionName { get; set; }
        public string? UserName { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormWorkFlowFormId { get; set; }
        public int? IsWorkFlowFormDone { get; set; } = 0;
        public int? DynamicFormWorkFlowFormTotalCount { get; set; }
        public int? DynamicFormWorkFlowFormCount { get; set; }
        public bool? IsAllowDelegateUser { get; set; } = false;
        public bool? IsAllowDelegateUserForm { get; set; } = false;
        public string? ActualUserName { get; set; }
        public long? ActualUserId { get; set; }
        public long? DelegateSectionUserId { get; set; }
        public string? DelegateSectionUserName { get; set; }
    }
    public class DynamicFormWorkFlowForm
    {
        [Key]
        public long DynamicFormWorkFlowFormId { get; set; }
        public long? DynamicFormWorkFlowSectionId { get; set; }
        public long? DynamicFormDataId { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public long? UserId { get; set; }
        public DateTime? CompletedDate { get; set; }
        [NotMapped]
        public string? CompletedBy { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Sequence No is required")]
        [DynamicFormWorkFormFlowSequenceNoCustomValidation]
        public int? SequenceNo { get; set; }
        [NotMapped]
        public long? DynamicFormSectionId { get; set; }
        [NotMapped]
        public long? DynamicFormWorkFlowId { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public long? DynamicFormWorkFlowUserId { get; set; }
        [NotMapped]
        public string? DynamicFormWorkFlowUser { get; set; }
        public int? RowID { get; set; }
        public int? FlowStatusID { get; set; }
        public bool? IsAllowDelegateUser { get; set; } = false;
        public bool? IsAllowDelegateUserForm { get; set; } = false;
        public string? ActualUserName { get; set; }
        public long? ActualUserId { get; set; }
        public long? DelegateSectionUserId { get; set; }
        public string? DelegateSectionUserName { get; set; }
        public int? DynamicFormWorkFlowFormTotalCount { get; set; }
        public int? DynamicFormWorkFlowFormCount { get; set; }
        public bool? DynamicFormWorkFlowApprovalFormCompleted { get; set; } = false;
        public long? DynamicFormId { get; set; }
        public List<long?> DynamicFormSectionIDs { get; set; } = new List<long?>();
        [DynamicFormWorkFormFlowSectionCustomValidation]
        public IEnumerable<long>? SelectDynamicFormSectionIDs { get; set; } = new List<long>();
        public long? CurrentApprovalUserId { get; set; }
        public string? CurrentApprovalUserName { get; set; }
        public long? NewDynamicFormWorkFlowFormId { get; set; }
        public bool? IsPendingApproval { get; set; } = false;
        public string? ProfileNo { get; set; }
        public string? FormName { get; set; }
        public bool? IsDelegateUser { get; set; } = false;
        public long? DelegateWorkFlowFormChangedId { get; set; }
        public Guid? FormSessionId { get; set; }
        public Guid? FormDataSessionId { get; set; }
    }
    public class DynamicFormDataUploadByPermission
    {
        public long? DynamicFormSectionId { get; set; }
        public bool? IsVisible { get; set; } = false;
        public bool? IsReadonly { get; set; } = false;
    }
    public class DynamicFormDataAll
    {
        public List<DynamicFormWorkFlowForm> DynamicFormWorkFlowForms { get; set; } = new List<DynamicFormWorkFlowForm>();
        public List<DynamicFormApproved> DynamicFormApproved { get; set; } = new List<DynamicFormApproved>();
        public List<DynamicFormWorkFlowSection> DynamicFormWorkFlowSection { get; set; } = new List<DynamicFormWorkFlowSection>();
        public List<DynamicFormSection> DynamicFormSection { get; set; } = new List<DynamicFormSection>();
    }
    public class DynamicFormWorkFlowItems
    {
        public List<DynamicFormWorkFlowForm> DynamicFormWorkFlowForm { get; set; } = new List<DynamicFormWorkFlowForm>();
        public List<DynamicFormWorkFlow> DynamicFormWorkFlow { get; set; } = new List<DynamicFormWorkFlow>();
        public List<DynamicFormWorkFlowSection> DynamicFormWorkFlowSection { get; set; } = new List<DynamicFormWorkFlowSection>();
        public List<DynamicFormWorkFlowApproval> DynamicFormWorkFlowApproval { get; set; } = new List<DynamicFormWorkFlowApproval>();


    }
    public class DynamicFormWorkFlowFormDelegate
    {
        public long DynamicFormWorkFlowFormDelegateID { get; set; }
        public long? DynamicFormWorkFlowFormID { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public long? UserID { get; set; }
        public string? UserName { get; set; }
    }
    public class DynamicFormWorkFlowSectionForm
    {
        public long DynamicFormWorkFlowSectionFormId { get; set; }
        public long? DynamicFormWorkFlowFormID { get; set; }
        public long? DynamicFormSectionID { get; set; }
        public string? SectionName { get; set; }
    }
}
