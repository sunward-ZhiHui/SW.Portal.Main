using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ForumConversations:BaseEntity
    {
        [Key]
        public int ID { get; set; }      
        public long TopicID { get; set; }
        public string Message { get; set; }
        public  long ParticipantId  { get; set; }
        public long ReplyId { get; set; }
    }
}
