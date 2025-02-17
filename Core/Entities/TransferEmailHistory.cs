using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TransferEmailHistory
    {
        [Key]
        public long ID { get; set; }
        public long FromId { get; set; }
        public long ToId { get; set; }
        public long TopicID { get; set; }
        public long ReplyId { get; set; }
        public long ConversationID { get; set; }
        public long UserID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime AddedDate { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? FromName { get; set; }
        [NotMapped]
        public string? ToName { get; set; }
        [NotMapped]
        public string? TransferBy { get; set; }
        [NotMapped]
        public string? TopicName { get;set; }
    }
}
