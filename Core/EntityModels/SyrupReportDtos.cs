using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Globalization;

namespace Core.EntityModels
{
    public class SyrupReportDtos
    {
        public class ProcessStepDto
        {
            public int Seq { get; set; }
            public long DynamicFormDataID { get; set; }
            public string Source { get; set; } = string.Empty;
            public string ProcessName { get; set; } = string.Empty;
            public string TaskName { get; set; } = string.Empty;
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string? Room { get; set; }
            public string? NextProcess_Timeline { get; set; }
            public string? Location { get; set; }
            public decimal? DurationHours { get; set; }
            public decimal? Manpower { get; set; }
            public string? NextProcess { get; set; }
            public string? Notes { get; set; }
            public int? WeekOfMonth { get; set; }
            public int? Month { get; set; }
            public int? Year { get; set; }
        }
        public class TimingDetailDto
        {
            public string ProcessName { get; set; } = string.Empty;
            public decimal? DurationHours { get; set; }
            public string? Manpower { get; set; }
            public string? Location { get; set; }
            public string? Notes { get; set; }
        }
        public class MachineInfoDto
        {
            public string Machine { get; set; } = string.Empty;
            public string? Type { get; set; }
            public string? Capacity { get; set; }
            public string? Speed { get; set; }
            public string? Notes { get; set; }
        }

        public class SyrupPlanningDto
        {
            public long SyrupPlanningID { get; set; }
            public string? ProfileNo { get; set; }
            public int? MethodCodeID { get; set; }
            // include other fields as needed by UI
        }
        public class ProductItemDto
        {
            public string No { get; set; } = string.Empty;          // NAV Item No (code)
            public string Description { get; set; } = string.Empty; // Product name
            public string? Description2 { get; set; }               // Optional secondary description
            public int? CategoryID { get; set; }                    // Product category ID (nullable)
        }
        public class TimingOverviewDto
        {
            public string? ProfileNo { get; set; }
            public string? MethodCodeName { get; set; }
            public string? WorkingHours { get; set; }        // e.g. "10 hrs"
            public string? WorkingHoursNote { get; set; }
            public string? FillingSpeed { get; set; }        // e.g. "60-85 bpm"
            public string? FillingSpeedNote { get; set; }
            public string? BatchSize { get; set; }           // e.g. "900 L"
            public string? BatchSizeNote { get; set; }
            public string? ShortNote { get; set; }
        }
        public class AppointmentData
        {
            public string? Subject { get; set; }
            public string Location { get; set; }
            public string Description { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public Nullable<bool> IsAllDay { get; set; }
            public string CategoryColor { get; set; }
            public string RecurrenceRule { get; set; }
            public Nullable<int> RecurrenceID { get; set; }
            public Nullable<int> FollowingID { get; set; }
            public string RecurrenceException { get; set; }
            public string StartTimezone { get; set; }
            public string EndTimezone { get; set; }
            public int Duration { get; set; } = 2;
        }
        public class TaskData
        {
            public int TaskId { get; set; }
            public string? TaskName { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public int Progress { get; set; }
            public string? Room { get; set; }
            public string? Predecessor { get; set; }
            public int? ParentId { get; set; }
            public int? SortChild { get; set; }
            public int? SortPlanning { get; set; }
            public int? SyrupPlanningID { get; set; }
            public string? Duration { get; set; }
            public decimal? DurationHours { get; set; }
            public string? NextProcessName { get; set; }
            public string? Notes { get; set; }
            public List<SegmentModel> Segments { get; set; } = new List<SegmentModel>();
        }
        public class SegmentModel
        {
            public int id { get; set; }
            public int TaskId { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string? Duration { get; set; } // decimal hours as string, e.g. "1.5"
        }

        public class SyrupResourceData 
        {
            public int Id { get; set; }
            public int? ParentId { get; set; }
            public string Subject { get; set; } = string.Empty;

            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }

            public bool IsAllDay { get; set; }
            public string? Type { get; set; }
            public string? Location { get; set; }
            public decimal? DurationHours { get; set; }
            public decimal? Manpower { get; set; }
            public string? Notes { get; set; }

            public int? Progress { get; set; }
            public int ChildId { get; set; }
            public dynamic ProfileNo { get; set; }
            public dynamic SyrupPlanningID { get; set; }
        }
    }

}
