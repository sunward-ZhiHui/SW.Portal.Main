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
    public class EmailTimelineEvent : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        public long DocumentID { get; set; }
        public long TopicID { get; set; }
        public long ConversationID { get; set; }

        [Required(ErrorMessage = "Please Enter Description.")]
        public string? Description { get; set; }
        [NotMapped]
        public string? UserName { get; set; }
    }
}
