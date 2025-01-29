using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailCopyLink
    {
        [Key] 
        public long EmailCopyLinkID { get; set; }
        public long? EmailConversationsId { get; set; }
        public Guid? EmailConversationsSessionid { get; set; }
        public Guid? SessionID { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; } = DateTime.Now;
    }
}
