using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TaskMaster
    {
        [Key]
        public long TaskId { get; set; }
        public long? MainTaskId { get; set; }
        public long? ParentTaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long? AssignedTo { get; set; }
        public DateTime? AssignedDate { get; set; }
        public DateTime? DueDate { get; set; }
        public long? ProjectId { get; set; }
        public long? OnBehalf { get; set; }
        public long? OwnerId { get; set; }
        public string FollowUp { get; set; }
        public string OverDueAllowed { get; set; }
        public bool? IsUrgent { get; set; }
        public DateTime? DiscussionDate { get; set; }
        public bool? IsRead { get; set; }
        public int? OrderNo { get; set; }
        public string Version { get; set; }
        public int? WorkTypeId { get; set; }
        public int? RequestTypeId { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string TaskNumber { get; set; }
        public int? TaskLevel { get; set; }
        public bool? IsNoDueDate { get; set; }
        public bool? IsAutoClose { get; set; }
        public bool? IsNotifyByMessage { get; set; }
        public bool? IsEmail { get; set; }
        public Guid? VersionSessionId { get; set; }
        public bool? IsAllowEditContent { get; set; }
        public DateTime? NewDueDate { get; set; }
        public long? SourceId { get; set; }
        public bool? IsFromProfileDocument { get; set; }
    }
}
