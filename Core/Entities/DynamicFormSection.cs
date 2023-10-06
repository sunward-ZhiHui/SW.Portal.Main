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
        [NotMapped]
        public int? SortOrderAnotherBy { get; set; }
        [NotMapped]
        public IEnumerable<AttributeHeader> AttributeIds { get; set; }=new List<AttributeHeader>();
    }
    public class DynamicFormSectionSortOrder
    {
        public int? Value { get; set; }
        public string? Text { get; set; }
       
    }

}
