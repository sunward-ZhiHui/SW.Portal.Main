using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicForm
    {
        [Key]
        public long ID { get; set; }
        public string Name { get; set; }
        [Required]
        public string ScreenID { get; set; }
       
        public string AttributeID { get; set; }
        public Guid SessionID { get; set; }
        [NotMapped]
        public IEnumerable<long> AttributeIds { get; set; }
        [NotMapped]
        public List<AttributeHeader>? _attributesName { get; set; }
    }
}
