using Core.Entities;

namespace SW.Portal.Solutions.Models
{
    public class EmailConversationViewModel
    {
        public long id { get; set; }
        public long replyId { get; set; }
        public long topicId { get; set; }
        public string topicName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        
        public List<EmailAssignToList>? To { get; set; }
        public List<EmailAssignToList>? CC { get; set; }        
        public byte[] message { get; set; }
        public long replyCount { get; set; }
        public List<ReplyConversationModel>? ReplyConversation { get; set; }
        public bool? urgent { get; set; }
        public bool? isAllowParticipants { get; set; }
        public DateTime? dueDate { get; set; }
        public long? onBehalf { get; set; }
        public string? onBehalfName { get; set; }
        public DateTime addedDate { get; set; }
        public long? addedByUserID { get; set; }
        public long userId { get; set; }
        public int? addedDateYear { get; set; }
        public string? addedDateDay { get; set; }
        public string? addedTime { get; set; }
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
