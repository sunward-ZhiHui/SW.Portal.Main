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
        public long? ActivityStatusId { get; set; }
        [NotMapped]
        public long DocumentID { get; set; }
        [NotMapped]
        public string? FileName { get; set; }
        [NotMapped]
        public string? ContentType { get; set; }
        [NotMapped]
        public bool? IsNewPath { get; set; }
        [NotMapped]
        public string? FilePath { get; set; }
        [NotMapped]
        public Guid? UniqueSessionId { get; set; }
        [NotMapped]
        public Guid? DocumentSessionId { get; set; }
        [NotMapped]
        public long? ManufacturingProcessChildId { get; set; }
        [NotMapped]
        public long? ProdActivityCategoryChildID { get; set; }
        [NotMapped]
        public long? ProdActivityActionChildD { get; set; }
        [NotMapped]
        public Guid? ActivitySessionID { get; set; }
        [NotMapped]
        public Guid? EmailTopicSessionId { get; set; }
        [NotMapped]
        public bool? IsPartialEmailCreated { get; set; }
        [NotMapped]
        public bool? IsEmailCreated { get; set; }
        [NotMapped]
        public bool? IsLocked { get; set; }
        [NotMapped]
        public long? LockedByUserId { get; set; }
        [NotMapped]
        public string? LockedByUser { get; set; }
    }
}
