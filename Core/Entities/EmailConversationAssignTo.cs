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
    public class EmailConversationAssignTo : BaseEntity
    {
        [Key]
        public long ID { get; set; }        
        public long ConversationId { get; set; }
        public long TopicId { get; set; }
        public long UserId { get; set; }
        [NotMapped]
        public string FirstName { get; set; }
        [NotMapped]
        public string LastName { get; set; }
        [NotMapped]
        public long? RowIndex { get; set; }
        [NotMapped]
        public string? AssigntoIds { get; set; }
        [NotMapped]
        public string? AssignccIds { get; set; }
    }
}
