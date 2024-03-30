using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailDueDateHistory
    {
        [Key]
        public long ID { get; set; }
        public long ConversationID { get; set; }
        public long UserID { get; set; }
        public DateTime DueDate { get; set; }
        public int NoOfDays { get; set; } = 0;
        public DateTime? ExtendDueDate { get; set; }
        public DateTime? ExpiryDueDate { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime AddedDate { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
    }
}
