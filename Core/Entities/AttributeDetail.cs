using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AttributeDetail : BaseEntity
    {
        [Key]
        public int AttributeDetailID { get; set; }
        public int AttributeID { get; set; }
        [Required(ErrorMessage = "Code is Required")]
        public string Code { get; set; }
        public string Description { get; set; }
        public int SortOrder { get; set; }
        public bool Disabled { get; set; }
    }
}
