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
    public class DynamicFormSectionAttribute : BaseEntity
    {
        [Key]
        public long DynamicFormSectionAttributeId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        [Required(ErrorMessage = "Label Name is Required")]
        public string? DisplayName { get; set; }
        [Required(ErrorMessage = "Field Name is Required")]
        public long? AttributeId { get; set; }

        public int? SortOrderBy { get; set; }
        public bool? IsMultiple { get; set; } = false;
        public bool? IsRequired { get; set; } = false;
        public bool? IsDisplayTableHeader { get; set; } = false;
        public string? RequiredMessage { get; set; }
        public string? IsSpinEditType { get; set; }
        public int? FormUsedCount { get; set; }
        public int? ColSpan { get; set; }
        [NotMapped]
        public string? SectionName { get; set; }
        [NotMapped]
        public string? AttributeName { get; set; }
        [NotMapped]
        public int? SortOrderAnotherBy { get; set; }
        [NotMapped]
        public string? ControlType { get; set; }
        [NotMapped]
        public int? ControlTypeId { get; set; }
        [NotMapped]
        public long? DynamicFormId { get; set; }
        [NotMapped]
        public Type? DataType { get; set; }
        [NotMapped]
        public string? DynamicAttributeName { get; set; }
        [NotMapped]
        public string? DropDownTypeId { get; set; }
        [NotMapped]
        public long? DataSourceId { get; set; }
        [NotMapped]
        public string? DataSourceDisplayName { get; set; }
        [NotMapped]
        public string? DataSourceTable { get; set; }

    }
    public class DynamicFormSectionSpinEdit
    {
        public string? Value { get; set; }
        public string? Text { get; set; }

    }
}
