using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentsUploadModel
    {

        public long? DocumentId { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public long? UserId { get; set; }
        public long? ProfileId { get; set; }
        public string? Type { get; set; }
        public Guid? UserSession { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "FileProfile Type is Required")]
        public long? FileProfileTypeId { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? PlantId { get; set; } = 0;
        [Required(ErrorMessage = "Department is Required")]
        public long? DepartmentId { get; set; } = 0;
        [Required(ErrorMessage = "Section is Required")]
        public long? SectionId { get; set; } = 0;
        [Required(ErrorMessage = "SubSection is Required")]
        public long? SubSectionId { get; set; } = 0;
        [Required(ErrorMessage = "Division is Required")]
        public long? DivisionId { get; set; } = 0;
        public string? CompanyCode { get; set; }
        public string? SectionName { get; set; }
        public string? SubSectionName { get; set; }
        public string? DepartmentName { get; set; }
        public string? ProfileNo { get; set; }
        public Guid? SessionId { get; set; }
        public long? DocumentParentId { get; set; }
        public long? DocumentMainParentId { get; set; }
        public bool? IsCheckOut { get; set; } = false;
        public string? FilePath { get; set; }
        public string? FileName { get; set; }
        public string? FileSizes { get; set; }
        public Guid? FileSessionId { get; set; }
        public string? SourceFrom { get; set; }
        public bool? IsSourceFrom { get; set; } = false;
        public string? TemplateUploadType { get; set; }
        [Required(ErrorMessage = "New File is Required")]
        public string? ChangeNewFileName { get; set; } = "New File Empty";
        public long? ProductionActivityAppLineId { get; set; }
        public long? ProductionActivityRoutineAppLineId { get; set; }
        public long? LinkDocumentId { get; set; }
        public List<DocumentsUploadModel> DocumentsUploadModels { get; set; } = new List<DocumentsUploadModel>();
        public List<DocumentsUploadModel> FailedDocumentsUploadModels { get; set; } = new List<DocumentsUploadModel>();
        public int? Index { get; set; }
        public List<DocumentsModel> DocumentIds { get; set; } = new List<DocumentsModel>();
        public DateTime StartDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Section is Required")]
        public long? DynamicFormSectionId { get; set; } = 0;
    }
}
