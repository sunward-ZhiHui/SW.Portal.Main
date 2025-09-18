using System.ComponentModel.DataAnnotations.Schema;
namespace DocumentViewer.Models
{
    public class EmailConversation
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public long TopicID { get; set; }
        public string? Message { get; set; }
        public DateTime? DueDate { get; set; }
        public bool? IsAllowParticipants { get; set; }
        public long? ParticipantId { get; set; }
        public long ReplyId { get; set; }        
        public bool IsLockDueDate { get; set; } = false;        
        public byte[] FileData { get; set; }
        public string? Description { get; set; }
        public DateTime AddedDate { get; set; }
        public int? IsMobile { get; set; }
        public bool? Urgent { get; set; } = false;
        public bool? NotifyUser { get; set; }
        public long? OnBehalf { get; set; }          
        public string? UserType { get; set; }            
        public int StatusCodeID { get; set; }
        public int? AddedByUserID { get; set; }
        public Guid? SessionId { get; set; }          
        public int IsDueDate { get; set; } = 0;      
        public long TransferID { get; set; }
    }       
}
