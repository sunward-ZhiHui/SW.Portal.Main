using Core.Entities.Base;
using Core.Entities.CustomValidations;
using Core.Entities.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Entities.CustomValidations.AttributeCustomValidation;

namespace Core.Entities
{
    public class AttributeHeader : BaseEntity
    {
        [Key]

        public long AttributeID { get; set; }
        //[Required(ErrorMessage = "Field Name is Required")]
       // [AttributeCustomValidation]
        public string AttributeName { get; set; }
        [Required(ErrorMessage = "Label Name is Required")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Company Name is Required")]
        public long? AttributeCompanyId { get; set; }
        public string ControlType { get; set; }
        public string ControlTypes { get; set; }
        public string EntryMask { get; set; }
        public string RegExp { get; set; }
        public string ListDefault { get; set; }
        public bool IsInternal { get; set; } = false;
        public string? AttrDescription { get; set; }
        public string? DropDownTypeId { get; set; } = null;
        [Required(ErrorMessage = "Data Source is Required")]
        public long? DataSourceId { get; set; }
        [NotMapped]
        public string? DataSourceDisplayName { get; set; }
        [NotMapped]
        public string? DataSourceTable { get; set; }
        public bool ContainsPersonalData { get; set; }
        //[NotMapped]
        //public string ModifiedBy { get; set; }
        //[NotMapped]
        //public string AddedBy { get; set; }
        [NotMapped]
        public int? RowIndex { get; set; }
        [Required(ErrorMessage = "Control Type is Required")]
        public int? ControlTypeId { get; set; }
        public bool? IsMultiple { get; set; } = false;
        public bool? IsRequired { get; set; } = false;
        public string? RequiredMessage { get; set; }
        public int? FormUsedCount { get; set; }
        [NotMapped]
        public bool? IsDataSource { get; set; } = false;
        [Required(ErrorMessage = "Grid Form is Required")]
        public long? DynamicFormId { get; set; }

        public bool? IsDynamicFormDropTagBox { get; set; } = false;
        public bool? IsSubForm { get; set; } = false;
        public long? SubAttributeId { get; set; }
        public long? SubAttributeDetailId { get; set; }
        public Type? SubDataType { get; set; }
        public string? SubDynamicAttributeName { get; set; }
        public List<AttributeDetails> SubAttributeDetails = new List<AttributeDetails>();
        public bool? AttributeIsVisible { get; set; } = false;
        public string? IsAttributeSpinEditType { get; set; }
        public bool? IsAttributeDisplayTableHeader { get; set; } = false;
        public string? AttributeFormToolTips { get; set; }
        public string? AttributeRadioLayout { get; set; }
        public string? AttributeCompany { get; set; }
        public string? SubFormType { get; set; }
        public long? ApplicationMasterSubFormId { get; set; }
        public bool? IsDeleted { get; set; }
        public string? SubApplicationMasterIDs { get; set; }
        public IEnumerable<long?> SubApplicationMasterIdsListIds { get; set; } = new List<long?>();
        public List<ApplicationMaster> SubApplicationMaster { get; set; } = new List<ApplicationMaster>();
    }
    public class AttributeHeaderListModel
    {
        public List<DynamicFormSection> DynamicFormSection { get; set; }
        public List<DynamicFormSectionAttribute> DynamicFormSectionAttribute { get; set; }
        public List<AttributeDetails> AttributeDetails { get; set; }
        public DropDownOptionsGridListModel DropDownOptionsGridListModel { get; set; } = new DropDownOptionsGridListModel();
        public List<Plant> Plant { get; set; } = new List<Plant>();
        public List<AttributeHeaderDataSource> AttributeHeaderDataSource { get; set; } = new List<AttributeHeaderDataSource>();
        public List<ApplicationMasterParent> ApplicationMasterParent { get; set; } = new List<ApplicationMasterParent>();
    }
    public class DynamicFormGridModel
    {
        public List<DynamicFormSectionAttribute> DynamicFormSectionAttribute { get; set; } = new List<DynamicFormSectionAttribute>();
        public List<DynamicFormData> DynamicFormData { get; set; } = new List<DynamicFormData>();
        public List<DynamicFormApproved> DynamicFormApproved { get; set; } = new List<DynamicFormApproved>();
        public List<AttributeDetails> AttributeDetails { get; set; } = new List<AttributeDetails>();
        public List<DynamicForm> DynamicForm { get; set; } = new List<DynamicForm>();
        public List<AttributeHeaderDataSource> AttributeHeaderDataSource { get; set; } = new List<AttributeHeaderDataSource>();
        public List<ApplicationMasterParent> ApplicationMasterParent { get; set; } = new List<ApplicationMasterParent>();
    }
    public class DropDownGridOptionsModel
    {
        public long? DynamicFormId { get; set; }
        public List<DropDownOptionsModel?> DropDownOptionsModels { get; set; } = new List<DropDownOptionsModel?>();
    }
    public class AttributeDetailsAdds
    {
        public List<AttributeDetails> AttributeDetails { get; set; } = new List<AttributeDetails>();
        public List<AttributeHeader> AttributeHeader { get; set; } = new List<AttributeHeader>();
        public List<ApplicationMaster> ApplicationMaster { get; set; } = new List<ApplicationMaster>();
    }
    public class DropDownOptionsModel
    {
        public string? Value { get; set; }
        public string? Text { get; set; }
        public long? Id { get; set; }
        public string? Type { get; set; }
        public bool IsVisible { get; set; } = true;
        public long? OrderBy { get; set; } = 0;
        public long AttributeDetailID { get; set; }
        public string? AttributeDetailName { get; set; }
        public long? DynamicFormId { get; set; }
    }
    public class DropDownOptionsGridListModel
    {
        public object? ObjectData { get; set; }
        public List<DropDownGridOptionsModel> DropDownGridOptionsModel { get; set; } = new List<DropDownGridOptionsModel>();
    }

}
