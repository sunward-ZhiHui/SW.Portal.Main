using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class JobScheduleWeekly
    {
        [Key]
        public long JobScheduleWeeklyId { get; set; }
        public long? JobScheduleId { get; set; }
        public int? WeeklyId { get; set; }
        public string? CustomType { get; set; }
        public string? Weekly { get; set; }
    }
}
