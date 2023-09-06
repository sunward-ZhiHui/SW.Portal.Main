using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class FileProfileSetupFormModel : BaseModel
    {
        public long FileProfileSetupFormId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public int? ControlTypeId { get; set; }
        public string ControlTypeValue { get; set; }
        public string DefaultValue { get; set; }
        public bool? IsDefault { get; set; }
        public DateTime? Date { get; set; }
        public List<string> DropDownValues { get; set; }
        public string ControlType { get; set; }
        public bool? IsMultiple { get; set; }
        public bool? IsRequired { get; set; }
        public string Placeholder { get; set; }
        public string RequiredMessage { get; set; }
        public string PropertyName { get; set; }
        public string DropDownTypeId { get; set; }
        public string DataSourceId { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public string? FileProfileTypeName { get; set; }
        public List<DropDownNameItems> DropDownItems { get; set; }

    }
    public class DropDownNameItems
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}
