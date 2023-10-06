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
        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Screen Name is Required")]
        [DynamicFormCustomValidation]
        public string ScreenID { get; set; }

        public string AttributeID { get; set; }
        public Guid SessionID { get; set; }
        [NotMapped]
        public IEnumerable<long> AttributeIds { get; set; }
        [NotMapped]
        public List<AttributeHeader>? _attributesName { get; set; }
    }
}
