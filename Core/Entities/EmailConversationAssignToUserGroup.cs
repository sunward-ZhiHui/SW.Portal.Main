using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{   
    public class EmailConversationAssignToUserGroup
    {
        [Key]
        public long ID { get; set; }
        public long GroupId { get; set; }
        public long ConversationId { get; set; }
        public long TopicId { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public long? ReplyId { get; set; }
        [NotMapped]
        public long ConIds { get; set; }
        [NotMapped]
        public string? AssigntoIds { get; set; }
        [NotMapped]
        public string? AssignccIds { get; set; }
        [NotMapped]
        public string? PlistIdss { get; set; }
        [NotMapped]
        public string? AllowPlistids { get; set; }
        [NotMapped]
        public string? Name { get;set; }
        [NotMapped]
        public string? SubjectName { get; set; }
        
    }
}
