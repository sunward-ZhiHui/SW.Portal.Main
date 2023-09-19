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
    public class AttributeHeader : BaseEntity
    {
        [Key]
       
        public int AttributeID { get; set; }
      // [Required]
        public string AttributeName { get; set; }
        [Required]
        public string Description { get; set; }

        public string ControlType { get; set; }

        public string EntryMask { get; set; }
        public string RegExp { get; set; }
        public string ListDefault { get; set; }
        public bool IsInternal { get; set; }
        public bool ContainsPersonalData { get; set; }
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        [NotMapped]
        public int? RowIndex { get; set; }

    }
}
