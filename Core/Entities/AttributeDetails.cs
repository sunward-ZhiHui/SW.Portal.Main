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
    public class AttributeDetails : BaseEntity
    {
        [Key]
        public long AttributeDetailID { get; set; }

        public string  AttributeDetailName { get; set; }
        public long? AttributeID { get; set; }
        [Required]
        public string Description { get; set; }
      
        public bool Disabled { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        
    }
}
