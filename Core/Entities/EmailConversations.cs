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
        public List<EmailDocumentModel>? documents { get; set; }
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
        public IEnumerable<long>? ToUserGroupIds { get; set; } = null;
        [NotMapped]
        public IEnumerable<long>? CCUserGroupIds { get; set; }
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
        public string? UserType { get; set; }
        public string? ToUserGroup { get; set; }
        public string? CCUserGroup { get; set; }
        public string? ParticipantsUserGroup { get; set; }
        [NotMapped]
        public string? DynamicFormName {  get; set; }
        [NotMapped]
        public int NoOfDays { get; set; }
        [NotMapped]
        public DateTime? ExpiryDueDate { get; set; }
        [NotMapped]
        public string? DynamicFormEmailSectionName { get; set; }
        
        [NotMapped]
        public Guid? EmailFormDataSessionID { get; set; }
        [NotMapped]
        public Guid? EmailFormSectionSessionID { get; set; }
        [NotMapped]
        public long? DynamicFormID { get; set; }


    }
    public class EmailDocumentModel
    {        
        public string? FileName { get; set; }
        public long DocumentID { get; set; }
        public Guid? SessionId { get; set; }
        public string? ContentType { get; set; }
        public string? FilePath { get; set; }
        public long? FileSize { get; set; }

    }
    public class ReplyConversationModel
    {
        public long id { get; set; }
        public long replyId { get; set; }
        public long participantId { get; set; }
        public string? userName { get; set; }
        public string? TopicName { get; set; }
        public string? firstName { get; set; }
        public byte[] Message { get; set; }
        public List<EmailAssignToList>? To { get; set; }
        public List<EmailAssignToList>? CC { get; set; }
        public long? userId { get; set; }
        public bool? isRead { get; set; }
        public long emailNotificationId { get; set; }
        public bool? urgent { get; set; }
        public DateTime? addedDate { get; set; }
        public Guid? sessionId { get; set; }
        public bool? isAllowParticipants { get; set; }
        public int? addedDateYear { get; set; }
        public string? addedDateDay { get; set; }
        public string? addedTime { get; set; }
    }
}
