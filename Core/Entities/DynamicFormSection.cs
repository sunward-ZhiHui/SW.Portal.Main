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
    public class DynamicFormSection : BaseEntity
    {
        [Key]
        public long DynamicFormSectionId { get; set; }
        [Required(ErrorMessage = "Section Name is required")]
        public string? SectionName { get; set; }

        public long? DynamicFormId { get; set; }

        public int? SortOrderBy { get; set; }
        public bool? IsReadWrite { get; set; }=false;
        public bool? IsReadOnly { get; set; } = false;

        public bool? IsVisible { get; set; } = false;   
        [NotMapped]
        public int? SortOrderAnotherBy { get; set; }
        [NotMapped]
        public IEnumerable<AttributeHeader> AttributeIds { get; set; }=new List<AttributeHeader>();
        [NotMapped]
        public int? FormUsedCount { get; set; }
        [NotMapped]
        public int? IsPermissionCount { get; set; }
        [NotMapped]
        public int? IsLoginUsers { get; set; }
    }
    public class DynamicFormSectionSortOrder
    {
        public int? Value { get; set; }
        public string? Text { get; set; }
       
    }

}
