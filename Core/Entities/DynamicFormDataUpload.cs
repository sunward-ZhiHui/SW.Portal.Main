using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormDataUpload
    {
        [Key]
        public long DynamicFormDataUploadId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public Guid? SessionId { get; set; }
        public Guid? EmailSessionId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public Guid? DynamicFormDataSessionId { get; set; }
        [NotMapped]
        public long? DynamicFormId { get; set; }
        [NotMapped]
        public Guid? DynamicFormSessionId { get; set; }
        [NotMapped]
        public string? FileName { get; set; }
        [NotMapped]
        public long? DocumentId { get; set; }
        [NotMapped]
        public Guid? FileProfileSessionID { get; set; }
        [NotMapped]
        public string? FileProfileName { get; set; }
        [NotMapped]
        public string? ProfileNo { get; set; }
        [NotMapped]
        public DocumentsModel? DocumentsModel { get; set; }
        public long? FileProfileTypeId { get; set; }
    }
}
