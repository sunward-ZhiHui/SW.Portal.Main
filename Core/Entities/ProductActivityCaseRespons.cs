using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductActivityCaseRespons
    {
        [Key]
        public long ProductActivityCaseResponsId { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public long? DutyId { get; set; }
        public string Responsibility { get; set; }
        public long? PageLink { get; set; }
        public long? FunctionLink { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? NotificationAdvice { get; set; }
        public int? NotificationAdviceTypeId { get; set; }
        public int? RepeatId { get; set; }
        public int? CustomId { get; set; }
        public DateTime? DueDate { get; set; }
        public int? Monthly { get; set; }
        public int? Yearly { get; set; }
        public string EventDescription { get; set; }
        public bool? DaysOfWeek { get; set; }
        public int? NotificationStatusId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public long? NotifyTo { get; set; }
        public string ScreenId { get; set; }
        public DateTime? NotifyEndDate { get; set; }
        public bool? IsAllowDocAccess { get; set; }
    }
}
