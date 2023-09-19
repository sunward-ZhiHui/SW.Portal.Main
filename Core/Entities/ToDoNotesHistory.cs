using Core.Entities.Base;
using Core.Entities.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ToDoNotesHistory : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        public long NotesId { get; set; }      
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? RemainDate { get; set; }
        public bool Completed { get; set; }
        public long? TopicId { get; set; }
        [NotMapped]
        public List<ViewEmployee>? participant { get; set; }
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }

        public string Status { get; set; }
        public string? ColourCode {get;set;}
        public string? Users { get; set; }
        [NotMapped]
        public IEnumerable<long> UserIds { get; set; }
        [NotMapped]
        public string SubjectName { get;set; }
    }
}
 