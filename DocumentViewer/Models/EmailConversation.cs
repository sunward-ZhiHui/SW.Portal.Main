using System.ComponentModel.DataAnnotations;
namespace DocumentViewer.Models
{
    public class EmailConversation
    {
        [Key]
        public int ID { get; set; }
        public long TopicId { get; set; }
        public Guid? SessionId { get; set; }
    }
}
