using Core.Entities.Base;
using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicForm:BaseEntity
    {
        [Key]
        public long ID { get; set; }
        [Required(ErrorMessage = "Form Name is Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Screen Name is Required")]
        [DynamicFormCustomValidation]
        public string? ScreenID { get; set; }
        [Required(ErrorMessage = "Profile is Required")]
        public long? ProfileId { get; set; }
        public bool? IsApproval { get; set; } = false;
        public string? AttributeID { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyId { get; set; }
        public bool IsUpload { get; set; } = false;
        public bool? IsMultipleUpload { get; set; } = false;
        //[Required]
        [DynamicFormApprovalIsUploadCustomValidation]
        public long? FileProfileTypeId { get; set; }
        public bool? IsProfileNoGenerate { get; set; } = false;
        public Guid SessionID { get; set; }
        [NotMapped]
        public IEnumerable<long> AttributeIds { get; set; }
        [NotMapped]
        public List<AttributeHeader>? AttributesName { get; set; }
        [NotMapped]
        public List<DynamicFormApproval>? DynamicFormApproval {  get; set; }
        [NotMapped]
        public string? FileProfileTypeName { get; set; }
        [NotMapped]
        public Guid? FileProfileSessionId { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        [NotMapped]
        public string? ProfileName { get; set; }
        public bool? IsGridForm { get; set; } = false;
    }
}
