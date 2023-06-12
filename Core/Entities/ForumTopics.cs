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
    public class ForumTopics : BaseEntity
    {
        [Key]
        public long ID { get; set; }        
        public string? TicketNo { get; set; }
        [Required(ErrorMessage = "Please Enter Subject Name.")]
        public string? TopicName { get; set; }       
        public long? TypeId { get; set; }
        public string? TypeName { get; set; }
        public long CategoryId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; } 
        public long TopicFrom { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Participants { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }       
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int SeqNo { get; set; }
        public string? SubjectName { get; set; }
        public string? Label { get; set; }
        public List<ForumTopics>? children { get; set; }
        public byte[] FileData { get; set; }      
        public string? Follow { get; set; }
        [Required(ErrorMessage = "Please Enter On Behalf of.")]
        public string? OnBehalf { get; set; }
        public bool? Urgent { get; set; }
        public bool? OverDue { get; set; }
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string? LastName { get; set; }

        [NotMapped]
        public string? From { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please Select To.")]
        public IEnumerable<long> ToIds { get; set; }
        [NotMapped]
        public IEnumerable<long> CCIds { get; set; }
        [NotMapped]
        public List<Documents>? documents { get; set; }
        [NotMapped]
        public List<ForumAssignToList>? TopicToList { get; set; }
        [NotMapped]
        public List<ForumAssignToList>? TopicCCList { get; set; }

    }
}
