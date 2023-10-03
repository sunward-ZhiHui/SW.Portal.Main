using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormData: BaseEntity
    {
        public long DynamicFormDataId { get; set; }
        public string? DynamicFormItem { get; set; }

        public long? DynamicFormId { get; set; }
    }
}
