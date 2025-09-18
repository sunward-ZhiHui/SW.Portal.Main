using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentViewer.Models
{
    public class EmailConversationAssignTo
    {
        public long ID { get; set; }
        public long ConversationId { get; set; }
        public long TopicId { get; set; }
        public long UserId { get; set; }       
        public int? AddedByUserID { get; set; }     
        public int StatusCodeID { get; set; }       
        public DateTime AddedDate { get; set; }     
        public Guid SessionId { get; set; }        
    }
}
