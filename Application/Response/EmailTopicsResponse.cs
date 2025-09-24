using Core.Entities;
using Core.Entities.Base;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class EmailTopicsResponse : BaseEntity
    {
        public long ID { get; set; }
        public string? TicketNo { get; set; }       
        public string? TopicName { get; set; }
        public long TypeId { get; set; }
        public long CategoryId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public long TopicFrom { get; set; }
        public string To { get; set; }
        public string CC { get; set; }        
        public string Participants { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
        public string? Priorities { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int SeqNo { get; set; }
        public string? SubjectName { get; set; }
        public string? label { get; set; }
        public byte[] FileData { get; set; }
        public string? Follow { get; set; }
        public long? OnBehalf { get; set; }
        public bool? Urgent { get; set; }
        public bool? NotifyUser { get; set; }
        public bool? TagLock { get; set; }
        public bool? OverDue { get; set; }
        public bool? IsAllowParticipants { get; set; }
        public int OnDraft { get; set; }
        public List<EmailTopicsResponse>? TopicList { get; set; }
        [NotMapped]
        public bool? isValidateSession { get; set; }
        [NotMapped]
        public long ActivityEmailTopicId { get; set;}
        public long? GroupTag { get; set; }
        public long? CategoryTag { get; set; }
        public long? ActionTag { get; set; }
        public IEnumerable<long?> ActionTagIds { get; set; } = new List<long?>();
        public IEnumerable<long?> UserTagIds { get; set; } = new List<long?>();
        public string? actName { get; set; }     
        public string? ActivityType { get; set;}
        public string? UserTag { get; set; }
        public long? UserTagId { get; set; }
        public string? UserType { get; set; }
        public string? ToUserGroup { get; set; }
        public string? CCUserGroup { get; set;}
        public string? ParticipantsUserGroup { get; set; }
        public int NoOfDays {  get; set; }
        public DateTime? ExpiryDueDate { get; set; }
        public bool? IsLockDueDate { get; set; }
        [NotMapped]
        public Guid? ActiveSessionId { get; set; }
        [NotMapped]
        public IEnumerable<Guid>? CopyEmailIds { get; set; } = new List<Guid>();
    }
}
