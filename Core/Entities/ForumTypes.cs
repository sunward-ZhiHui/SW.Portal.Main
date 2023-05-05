using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
