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
    public class ToDoNotes : BaseEntity
    {
        [Key]
        public long ID { get; set; }       
        public string? Notes { get; set; }  
        public string? Completed { get; set; }
        public long? TopicId { get; set; }
        
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        [NotMapped]
        public string? MainTopic { get; set; }
        [NotMapped]
        public string? SubTopic { get; set; }
    }
}
