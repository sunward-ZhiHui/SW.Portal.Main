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
        public string?  AttributeDetailName { get; set; }
        public long? AttributeID { get; set; }
        public string? Description { get; set; }
      
        public bool Disabled { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        public int? FormUsedCount { get; set; }
        [NotMapped]
        public object? DataSourceDetails { get; set; }
        [NotMapped]
        public string? DropDownTypeId { get; set; }

    }
}
