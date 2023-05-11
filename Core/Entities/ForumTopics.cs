using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ForumTopics : BaseEntity
    {
        [Key]
        public long ID { get; set; }        
        public string? TicketNo { get; set; }
        [Required(ErrorMessage = "Please Enter Topic Name.")]
        public string? TopicName { get; set; }
        [Required(ErrorMessage = "Please Select Type Name.")]
        public long? TypeId { get; set; }
        public long CategoryId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; } 
        public long TopicFrom { get; set; }
        public string To { get; set; }
        public List<long>? CC { get; set; }
        public List<long>? Participants { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int SeqNo { get; set; }
        public string? SubjectName { get; set; }

    }
}
