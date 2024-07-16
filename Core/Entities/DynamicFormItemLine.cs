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
    public class DynamicFormItemLine
    {
        [Key]
        public long DynamicFormItemLineID { get; set; }
        public long? DynamicFormItemID { get; set; }
        [Required(ErrorMessage = "Qty is Required")]
        public decimal? Qty { get; set; }
        [Required(ErrorMessage = "Form Type is Required")]
        public long? ItemDynamicFormTypeID { get; set;}
        [Required(ErrorMessage = "Form Data is Required")]
        public long? ItemDynamicFormDataID { get; set; }
        public string? Description { get;set; }
        [NotMapped]
        public string? ItemDynamicFormType { get; set; }
        [NotMapped]
        public string? ItemDynamicFormData { get; set; }
        public long? AddedByUserID { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? StatusCodeID { get; set; }
        public Guid? SessionId { get; set; }
    }
}
