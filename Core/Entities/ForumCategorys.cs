using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ForumCategorys : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public long TypeId { get; set; }
    }
}
