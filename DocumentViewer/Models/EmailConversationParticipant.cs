using System.ComponentModel.DataAnnotations;
namespace DocumentViewer.Models
{
    public class EmailConversationParticipant
    {
        [Key]
        public long ID { get; set; }
        public long TopicId { get; set; }
        public long UserId { get; set; }
        public Guid? SessionId { get; set; }
    }
}
