using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormDataAttrUpload
    {
        public long DynamicFormDataAttrUploadId { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public byte[]? ImageData { get; set; }
        public string? ImageType { get; set; }
        public string? FileName { get; set; }
        public decimal? FileSize { get; set; }
        public string? FileSizes { get; set; }
        public DateTime? UploadDate { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public Guid? SessionId { get; set; }
        public long? UploadedUserId { get; set; }
        public string? UploadedUser { get; set; }
        public string? DisplayName { get; set; }
        public int? UniqueId { get; set; }
        public string? FilePath { get; set; }
        public Guid? DynamicFormSectionAttributeSessionId { get; set; } 
    }
    public class CarouselData
    {
        public string? Source { get; set; }
        public string? AlternateText { get; set; }
        public string? Type { get; set; }
        public string? FileType { get; set; }
        public int Index { get; set; }
    }
}
