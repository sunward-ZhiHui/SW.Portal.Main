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
    public class DynamicFormData: BaseEntity
    {
        public long DynamicFormDataId { get; set; }
        public string? DynamicFormItem { get; set; }
        public bool? IsSendApproval { get; set; } = false;
        public long? DynamicFormId { get; set; }
        [NotMapped]
        public AttributeHeaderListModel? AttributeHeader { get; set; }
        [NotMapped]
        public object? ObjectData { get; set; }
        [NotMapped]
        public string? DynamicFormCurrentItem { get; set; }
        [NotMapped]
        public string? Name { get; set; }
        [NotMapped]
        public string? ScreenID { get; set; }
        [NotMapped]
        public bool? IsApproval { get; set; }
    }
}
