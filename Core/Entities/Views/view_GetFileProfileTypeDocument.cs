using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class view_GetFileProfileTypeDocument
    {
        public long FileProfileTypeId { get; set; }
        public long ProfileId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public int StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsExpiryDate { get; set; }
        public bool? IsAllowMobileUpload { get; set; }
        public bool? IsDocumentAccess { get; set; }
        public int? ShelfLifeDuration { get; set; }
        public int? ShelfLifeDurationId { get; set; }
        public string Hints { get; set; }
        public bool? IsEnableCreateTask { get; set; }
        public string ProfileTypeInfo { get; set; }
        public bool? IsCreateByYear { get; set; }
        public bool? IsCreateByMonth { get; set; }
        public bool? IsHidden { get; set; }
        public string ProfileInfo { get; set; }
        public bool? IsTemplateCaseNo { get; set; }
        public long? TemplateTestCaseId { get; set; }
        public string Profile { get; set; }
        public string FileProfileTypeName { get; set; }
        public string FileProfileDescription { get; set; }
        public string StatusCode { get; set;}
        public string AddedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string DocumentName { get; set; }
        public string DocFileSize { get; set; }
        public string DocumentDescription { get; set; }
        public string ProfileNo { get; set; }
        public string ExpiryDate { get; set; }
        public string ShelfLifeDurationStatus { get; set; }
        public string DocAddedDate { get; set; }
        public string ContentType { get; set;}
        public string FolderID { get; set; }
        public string DocAddedBy { get; set; }
        public Guid DocSessionID { get; set;}
    }
}
