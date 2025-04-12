using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class FileProfileTypeModel : BaseModel
    {
        public long FileProfileTypeId { get; set; }
        [Required(ErrorMessage = "Profile is Required")]
        public long? ProfileId { get; set; }
        [Required(ErrorMessage = "Name is Required")]
       // [FileProfileTypeNameCustomValidation]
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
        public bool? IsExpiryDate { get; set; } = false;
        public string ProfileName { get; set; }
        public bool? IsAllowMobileUpload { get; set; }
        public bool? IsDocumentAccess { get; set; }
        public List<FileProfileTypeModel> Children { get; set; }
        public DocumentPermissionModel DocumentPermissionData { get; set; }
        public long TotalDocuments { get; set; }
        public string FileProfilePath { get; set; }
        public string Hints { get; set; }
        public bool? IsEnableCreateTask { get; set; }
        public long? Id { get; set; }
        public List<string> FileProfilePaths { get; set; } = new List<string>();
        public List<string> FileProfileNames { get; set; } = new List<string>();
        public int? ShelfLifeDuration { get; set; }
        public string ShelfLifeDurationStatus { get; set; }
        public int? ShelfLifeDurationId { get; set; }
        public string ProfileTypeInfo { get; set; }
        public bool? IsCreateByYear { get; set; }
        public bool? IsCreateByMonth { get; set; }
        public bool? IsHidden { get; set; }
        public long? FileProfileTypeParentId { get; set; }
        public long? FileProfileTypeMainId { get; set; }
        public string ProfileInfo { get; set; }
        public bool? IsTemplateCaseNo { get; set; }
        public long? TemplateTestCaseId { get; set; }
        public long? DynamicFormId { get; set; }
        public int? IsdynamicFormExits { get; set; } = 0;
        public bool? IsDuplicateUpload { get; set; } = false;
        public bool? IsAllowWaterMark { get; set; } = false;
    }
}
