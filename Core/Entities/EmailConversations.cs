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
    public class EmailConversations:BaseEntity
    {
        [Key]
        public int ID { get; set; }      
        public long TopicID { get; set; }
        [Required(ErrorMessage = "Please Enter Message.")]
        public string Message { get; set; }
        public  long ParticipantId  { get; set; }
        public long ReplyId { get; set; }
        public string? ReplyMessage { get; set; }
        public DateTime? ReplyDateTime { get; set; }
        public string? ReplyUserName { get; set; }
        public List<EmailConversations>? ReplyConversation { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserCode { get; set; }
        public string UserEmail { get; set; }
        public List<Documents>? documents { get; set; }
        public byte[] FileData { get; set; }

        [NotMapped]
        public IEnumerable<long> AssigntoIds { get; set; }
        [NotMapped]
        public IEnumerable<long> AllParticipantIds { get; set; }
        [NotMapped]
        public List<EmailAssignToList>? AssignToList { get; set; }

    }
}
