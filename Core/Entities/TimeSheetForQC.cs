using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class TimeSheetForQC:BaseEntity
    {
        [Key]
        public long QCTimesheetID { get; set; }
        public string? ItemName { get; set; }
        public string? RefNo { get; set; }
        public string? Stage { get; set; }
        public string? TestName { get; set; }
        public string? DetailEntry { get; set; }
        public string? Comment { get; set; }
        public bool? QRcode { get; set; }
    }
}
