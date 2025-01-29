using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormSectionAttributeSection
    {
        [Key]
        public long DynamicFormSectionAttributeSectionId { get; set; }
        public long? DynamicFormSectionAttributeSectionParentId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public long? DynamicFormSectionSelectionId { get; set; }
        public string? DynamicFormSectionSelectionById { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        public int? SequenceNo { get;set; }
        public string? DisplayName { get; set; }
        public string? UserName { get; set; }
        public string? DynamicFormSectionSelectionBySectionName { get; set; }
        public string? SectionName { get; set; }
        public string? FormName { get; set; }
    }
    public class DynamicFormSectionAttributeSectionParent
    {
        [Key]
        public long DynamicFormSectionAttributeSectionParentId { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        public string? DynamicSectionName { get; set; }
        public string? ValueName { get; set; }
        public List<DynamicFormSectionAttributeSection> DynamicFormSectionAttributeSections { get; set; } = new List<DynamicFormSectionAttributeSection>();
        [Required(ErrorMessage = "Name is required")]
        public IEnumerable<string?> ShowSectionVisibleDataIds { get; set; } = new List<string?>();
        [Required(ErrorMessage = "Section Name is required")]
        public IEnumerable<long?> DynamicFormSectionIds { get; set; } = new List<long?>();
        public int? SequenceNo { get; set; }
        public long? DynamicFormId { get; set; }
        public string? FormName { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public string? SectionName { get; set; }
        public string? DisplayName { get; set; }
        public long? DataSourceId { get; set; }
        public string? DataSourceTable { get; set; }
        public long? AttributeId { get; set; }
        public int? ControlTypeId { get; set; }
        public string? DropDownTypeId { get; set; }
        public string? ApplicationMasterIds { get; set; }
    }
}
