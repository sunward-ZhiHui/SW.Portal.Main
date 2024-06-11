using Core.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DynamicFormReportItems
    {
        public string? AttrId { get; set; }
        public string? Label { get; set; }
        public string? Value { get; set; }
        public bool? IsGrid { get; set; } = false;
        public Guid? DynamicFormSessionId { get; set; }
        public Guid? DynamicFormDataSessionId { get; set; }
        public Guid? DynamicFormDataGridSessionId { get; set; }
        public string? Url { get; set; } = string.Empty;
        public List<DynamicFormData>? GridItems { get; set; } =new List<DynamicFormData>();
        public bool? IsSubForm { get; set; } = false;
        public List<dynamic>? GridSingleItems { get; set; } = new List<dynamic>();
    }
}
