using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TopicToDoList:BaseEntity
    {
        [Key]
        public long ID { get; set; }
        public long TopicId { get; set; }
       
        public byte[] ToDoName { get; set; }
        public bool Iscompleted { get; set; }
    }
}
