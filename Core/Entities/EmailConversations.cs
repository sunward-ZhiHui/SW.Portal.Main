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
        public string Name { get; set; }
        public long TopicID { get; set; }
        [Required(ErrorMessage = "Please Enter Message.")]
        public string Message { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsAllowParticipants { get; set; }
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
        public int? IsMobile { get; set; }
        [NotMapped]
        public string? AssigntoIdss { get; set; }
        [NotMapped]
        public string? AssignccIdss { get; set; }
        [NotMapped]
        public string? PlistIdss { get; set; }
        [NotMapped]
        public string? AllowPlistids { get; set; }        
        [NotMapped]
        public IEnumerable<long>? AssigntoIds { get; set; }
        [NotMapped]
        public IEnumerable<long> AssignccIds { get; set; }
        [NotMapped]
        public IEnumerable<long> AllParticipantIds { get; set; }
        [NotMapped]
        public List<EmailAssignToList>? AssignToList { get; set; }
        [NotMapped]
        public List<EmailAssignToList>? AssignCCList { get; set; }
        [NotMapped]
        public long? UserId { get; set; }
        [NotMapped]
        public string From { get; set; }
        [NotMapped]
        public long ConIds { get; set; }
        [NotMapped]
        public string? BackURL { get; set; }
        [NotMapped]
        public Guid? DocumentSessionId { get; set; }
        [NotMapped]
        public string? ActCommentName { get; set; }
        [NotMapped]
        public string? ActUserName { get; set; }
        [NotMapped]
        public DateTime? ActAddedDate { get; set; }
        [NotMapped]
        public string? ActivityType { get; set; }
        [NotMapped]
        public bool? IsRead { get; set; }
        [NotMapped]
        public long EmailNotificationId { get; set; }
        [NotMapped]
        public string? OnBehalfName { get; set; }
        [NotMapped]
        public string? Follow { get; set; }
        public bool? Urgent { get; set; } = false;
        public bool? NotifyUser { get; set; }
        public long? OnBehalf { get; set; }
        [NotMapped]
        public string? TopicName { get; set; }
        [NotMapped]
        public string? EmailConversationName { get; set; }
        [NotMapped]
        public long? ConversationId { get; set; }

    }
}
