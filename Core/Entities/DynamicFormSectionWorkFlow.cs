using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormSectionWorkFlow
    {
        [Key]
        public long DynamicFormSectionWorkFlowId { get; set; }
        public long? DynamicFormSectionSecurityId { get; set; }
        public int? UserId { get; set; }
        public long? SortOrderBy { get; set; }
        public string? DynamicFormSectionId { get; set; }
        public string? SectionName { get; set; }
        public string? DynamicFormSectionWorkFlowUserName { get; set; }
    }
}
