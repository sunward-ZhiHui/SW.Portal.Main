using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    }
}
