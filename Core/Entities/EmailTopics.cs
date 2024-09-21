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
    public class EmailTopics : BaseEntity
    {
        [Key]
        public long ID { get; set; }        
        public string? TicketNo { get; set; }
        [Required(ErrorMessage = "Please Enter Subject Name.")]
        public string? TopicName { get; set; }       
        public long? TypeId { get; set; }
        public string? UserType { get; set; }
        public string? TypeName { get; set; }
        public long CategoryId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; } 
        public long TopicFrom { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string Participants { get; set; }
        [NotMapped]
        public string? ToUserGroup { get; set; }
        [NotMapped]
        public string? CCUserGroup { get; set; }
        [NotMapped]
        public string? ParticipantsUserGroup { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }       
        public string? Description { get; set; }
        public string? Type { get; set; }
        public string? PinStatus { get; set; }
        public int SeqNo { get; set; }
        public string? SubjectName { get; set; }
        public string? Label { get; set; }
        public List<ForumTopics>? children { get; set; }
        public byte[] FileData { get; set; }      
        public string? Follow { get; set; }        
        public long? OnBehalf { get; set; }
        [NotMapped]
        public string? OnBehalfName { get; set; }
        public bool? Urgent { get; set; }
        public bool? NotifyUser { get; set; }
        public bool? TagLock { get; set; }
        public bool? OverDue { get; set; }
        public bool? IsArchive { get; set; }
        [NotMapped]
        public bool? IsToDoDuDate { get; set; }
        public bool? IsAllowParticipants { get; set; }
        public int OnDraft { get; set; }
        [NotMapped]
        public string? FirstName { get; set; }
        [NotMapped]
        public string? LastName { get; set; }
        [NotMapped]
        public string? Name { get; set; }
        [NotMapped]
        public string? NickName { get; set; }

        [NotMapped]
        public string? From { get; set; }

        [NotMapped]
        [Required(ErrorMessage = "Please Select To.")]
        public IEnumerable<long>? ToIds { get; set; } = null;
      //  public IEnumerable<ViewEmployee>? ToEmployeeIds { get; set; }

        [NotMapped]        
        public IEnumerable<long>? ToUserGroupIds { get; set; } = null;

        [NotMapped]
        public IEnumerable<long>? CCIds { get; set; } = null ;
       // public IEnumerable<ViewEmployee>? ccEmployeeIds { get; set; }

        [NotMapped]
        public IEnumerable<long>? CCUserGroupIds { get; set; }
        
        [NotMapped]
        public List<Documents>? documents { get; set; }
        [NotMapped]
        public List<EmailAssignToList>? TopicToList { get; set; }
        [NotMapped]
        public List<EmailAssignToList>? TopicCCList { get; set; }
        [NotMapped]
        public int? NotificationCount { get; set; }
        [NotMapped]
        public long ReplyId { get; set; }
        [NotMapped]
        public long? UserId { get; set; }

        [NotMapped]
        public bool? isValidateSession { get; set; }
        [NotMapped]
        public long ActivityEmailTopicId { get; set; }
        [NotMapped]
        public long EmailCount { get; set; }
        [NotMapped]
        public long GroupTag { get; set; }
        [NotMapped]
        public long CategoryTag { get; set; }
        [NotMapped]
        public long ActionTag { get; set; }
        [NotMapped]
        public IEnumerable<long?> ActionTagIds { get; set; } = new List<long?>();
        [NotMapped]
        public string? actName { get; set; }
        [NotMapped]
        public string? ActivityType { get; set;}
        [NotMapped]
        public string? UserTag { get; set; }
        [NotMapped]
        public string? OtherTag { get; set; }
        [NotMapped]
        public long? UserTagId { get; set; }
        [NotMapped]
        public string? AssignFrom { get; set; }
        [NotMapped]
        public string? AssignTo { get; set; }

        [NotMapped]
        public byte[]? Replymsg { get; set; }
        [NotMapped]
        public int NoOfDays { get; set; }
        [NotMapped]
        public long EmailTopicID { get; set; }
        [NotMapped]
        public DateTime? ExpiryDueDate { get; set; }

    }
}
