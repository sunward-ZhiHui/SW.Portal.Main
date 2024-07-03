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
    public class DynamicFormItemLine:BaseEntity
    {
        [Key]
        public long DynamicFormItemLineID { get; set; }
        public long? DynamicFormItemID { get; set; }
        public decimal Qty { get; set; }
        public long? ItemDynamicFormTypeID { get; set;}
        public long? ItemDynamicFormDataID { get; set; }
        public string Description { get;set; }
        [NotMapped]
        public string? ItemDynamicFormType { get; set; }
        [NotMapped]
        public string? ItemDynamicFormData { get; set; }
    }
}
