using Core.Entities.Base;
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
    public class DynamicForm:BaseEntity
    {
        [Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Form Name is Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Screen Name is Required")]
        [DynamicFormCustomValidation]
        public string? ScreenID { get; set; }
        [Required(ErrorMessage = "Profile is Required")]
        public long? ProfileId { get; set; }
        public bool? IsApproval { get; set; } = false;
        public string? AttributeID { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public bool IsUpload { get; set; } = false;
        public bool? IsMultipleUpload { get; set; } = false;
        //[Required]
        [DynamicFormApprovalIsUploadCustomValidation]
        public long? FileProfileTypeId { get; set; }
        public bool? IsProfileNoGenerate { get; set; } = false;
        public Guid SessionID { get; set; }
        [NotMapped]
        public IEnumerable<long> AttributeIds { get; set; }
        [NotMapped]
        public List<AttributeHeader>? AttributesName { get; set; }
        [NotMapped]
        public List<DynamicFormApproval>? DynamicFormApproval {  get; set; }
        [NotMapped]
        public string? FileProfileTypeName { get; set; }
        [NotMapped]
        public Guid? FileProfileSessionId { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? ProfileName { get; set; }
        public bool? IsGridForm { get; set; } = false;
        public int? FormDataCount { get; set; }
        public long? CloneDynamicFormId { get; set; }
        public long? OldDynamicFormId { get; set; }
        public int? SortBy { get; set; }
        public string? SopNo { get; set; }
        public string? VersionNo { get; set; }
    }
    public class AddTempSectionAttribute
    {
        public long? ID { get; set; }
        public string? DynamicAttributeName { get; set; }
        public Type? DataType { get; set; }
        public string? DynamicAttributeFieldName { get; set; }
        public string? UniqueId { get; set; }
    }
    public class DynamicFormDataListItems
    {
        public DynamicForm DynamicForm { get; set; } = new DynamicForm();
        public List<DynamicFormSection> DynamicFormSection { get; set; } = new List<DynamicFormSection>();
        public List<DynamicFormApproval> DynamicFormApproval { get; set; } = new List<DynamicFormApproval>();
        public List<DynamicFormWorkFlow> DynamicFormWorkFlow { get; set; } = new List<DynamicFormWorkFlow>();
        public List<DynamicFormWorkFlowSection> DynamicFormWorkFlowSection { get; set; } = new List<DynamicFormWorkFlowSection>();
        public List<DynamicFormWorkFlowApproval> DynamicFormWorkFlowApproval { get; set; } = new List<DynamicFormWorkFlowApproval>();
        public List<DynamicFormSectionAttribute> DynamicFormSectionAttribute { get; set; } = new List<DynamicFormSectionAttribute>();
        public List<DynamicFormSectionAttributeSectionParent> DynamicFormSectionAttributeSectionParent { get; set; } = new List<DynamicFormSectionAttributeSectionParent>();
        public List<DynamicFormSectionAttributeSection> DynamicFormSectionAttributeSection { get; set; } = new List<DynamicFormSectionAttributeSection>();
        public List<DynamicFormSectionSecurity> DynamicFormSectionSecurity { get; set; } = new List<DynamicFormSectionSecurity>();
        public List<DynamicFormSectionAttributeSecurity> DynamicFormSectionAttributeSecurity { get; set; } = new List<DynamicFormSectionAttributeSecurity>();

        public List<DynamicFormData> DynamicFormData { get; set; } = new List<DynamicFormData>();
        public List<DynamicFormData> DynamicFormDataGrid { get; set; } = new List<DynamicFormData>();
        public List<DynamicFormApproved> DynamicFormApproved { get; set; } = new List<DynamicFormApproved>();
        public List<DynamicFormApprovedChanged> DynamicFormApprovedChanged { get; set; } = new List<DynamicFormApprovedChanged>();
        public List<DynamicFormDataSectionLock> DynamicFormDataSectionLock { get; set; } = new List<DynamicFormDataSectionLock>();
        public List<DynamicFormWorkFlowForm> DynamicFormWorkFlowForm { get; set; } = new List<DynamicFormWorkFlowForm>();
        public List<DynamicFormWorkFlowSectionForm> DynamicFormWorkFlowSectionForm { get; set; } = new List<DynamicFormWorkFlowSectionForm>();
        public List<DynamicFormWorkFlowFormDelegate> DynamicFormWorkFlowFormDelegate { get; set; } = new List<DynamicFormWorkFlowFormDelegate>();
        public List<DynamicFormWorkFlowApprovedForm> DynamicFormWorkFlowApprovedForm { get; set; } = new List<DynamicFormWorkFlowApprovedForm>();
        public List<DynamicFormWorkFlowApprovedFormChanged> DynamicFormWorkFlowApprovedFormChanged { get; set; } = new List<DynamicFormWorkFlowApprovedFormChanged>();

        public string? DynamicFormDataIds{ get; set; }
    }
}
