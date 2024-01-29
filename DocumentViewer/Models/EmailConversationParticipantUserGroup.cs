using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class EmailConversationParticipantUserGroup
    {
        [Key]
        public int ID { get; set; }
        public long GroupId { get; set; }
        public long ConversationId { get; set; }
        public long TopicId { get; set; }
    }
}
