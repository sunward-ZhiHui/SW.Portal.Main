using System.ComponentModel.DataAnnotations;

namespace DocumentViewer.Models
{
    public class EmailNotifications
    {
        [Key]
        public long ID { get; set; }
        public long ConversationId { get; set; }
        public long TopicId { get; set; }
        public long UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime AddedDate { get; set; }
        public long? AddedByUserID { get; set; }
    }
}
