using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentNoSeries
    {
        [Key]
        public long NumberSeriesId { get; set; }
        public long? ProfileId { get; set; }
        public string DocumentNo { get; set; }
        public string VersionNo { get; set; }
        public string Title { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? Implementation { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? NextReviewDate { get; set; }
        public DateTime? Date { get; set; }
        public long? RequestorId { get; set; }
        public string Link { get; set; }
        public string ReasonToVoid { get; set; }
        public string Description { get; set; }
        public Guid? SessionId { get; set; }
        public bool? IsUpload { get; set; }
        public long? FileProfileTypeId { get; set; }
        [NotMapped]
        public string? ProfileName { get; set; }
        [NotMapped]
        public string? AddedBy { get; set; }
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? RequestorName { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? DepartmentName { get; set; }
        [NotMapped]
        public string? FileprofileTypeName { get; set; }
        [NotMapped]
        public string? FullPath { get; set; }
        [NotMapped]
        public long? FileProfileTypeParentId { get; set; }
        [NotMapped]
        public bool? IsDeletedSWProfileType { get; set; }=false;
    }
}
