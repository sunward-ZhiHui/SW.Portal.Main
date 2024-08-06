using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class JobSchedule
    {
        [Key]
        public long JobScheduleId { get; set; }
        [Required(ErrorMessage = "Frequency is Required")]
        public int? FrequencyId { get; set; }
        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime? StartDate { get; set; }
        [Required(ErrorMessage = "Execution Time is Required")]
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MonthlyDay { get; set; }
        public bool? DaysOfWeek { get; set; } = false;
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? StatusCode { get; set; }
        public string? AddedByUser { get; set; }
        public string? ModifiedByUser { get; set; }
        public IEnumerable<int?> DaysOfWeekIds { get; set; } = new List<int?>();
        public IEnumerable<int?> NoticeWeeklyIds { get; set; } = new List<int?>();
        public string? Frequency { get; set; }
        public string? CompanyCode { get; set; }
        [Required(ErrorMessage = "Function Name is Required")]
        public long? JobScheduleFunUniqueId { get; set; }
        public string? JobScheduleFunUnique { get; set; }
    }
}
