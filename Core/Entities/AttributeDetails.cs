using Core.Entities.Base;
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
    public class AttributeDetails : BaseEntity
    {
        [Key]
        public long AttributeDetailID { get; set; }
        [Required(ErrorMessage = "Value is Required")]
        [AttributeValueCustomValidation]
        public string? AttributeDetailName { get; set; }
        public long? AttributeID { get; set; }
        public string? Description { get; set; }

        public bool Disabled { get; set; } = false;
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        public int? FormUsedCount { get; set; }
        [NotMapped]
        public object? DataSourceDetails { get; set; }
        [NotMapped]
        public string? DropDownTypeId { get; set; }
        public long? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public List<AttributeHeader> SubAttributeHeaders { get; set; } = new List<AttributeHeader>();
        public long? ApplicationMasterId { get; set; }
        public string? ApplicationMasterName { get; set; }
        public long? ApplicationMasterCodeId { get; set; }
    }
    public class DataSourceAttributeDetails
    {
        public List<AttributeDetails> AllAttributeDetails { get; set; } = new List<AttributeDetails>();
    }
}
