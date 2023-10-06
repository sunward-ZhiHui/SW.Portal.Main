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

namespace Core.Entities
{
    public class AttributeHeader : BaseEntity
    {
        [Key]

        public long AttributeID { get; set; }
        [Required(ErrorMessage = "Attribute Name is Required")]
        [AttributeCustomValidation]
        public string AttributeName { get; set; }
        [Required(ErrorMessage = "Label Name is Required")]
        public string Description { get; set; }

        public string ControlType { get; set; }

        public string EntryMask { get; set; }
        public string RegExp { get; set; }
        public string ListDefault { get; set; }
        public bool IsInternal { get; set; } = false;
        public bool ContainsPersonalData { get; set; }
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        [NotMapped]
        public int? RowIndex { get; set; }
        [Required(ErrorMessage = "Control Type is Required")]
        public int? ControlTypeId { get; set; }
        public bool? IsMultiple { get; set; } = false;
        public bool? IsRequired { get; set; } = false;
        public bool? RequiredMessage { get; set; }
        public int? FormUsedCount { get; set; }
    }
    public class AttributeHeaderListModel
    {
        public List<DynamicFormSection> DynamicFormSection { get; set; }
        public List<DynamicFormSectionAttribute> DynamicFormSectionAttribute { get; set; }
        public List<AttributeDetails> AttributeDetails { get; set; }
    }

}
