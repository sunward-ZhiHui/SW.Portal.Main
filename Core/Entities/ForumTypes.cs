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
    public class ForumTypes : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Please Enter Name.")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        [NotMapped]
        public int? RowIndex { get; set; }
    }
}
