using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormApproved
    {
        [Key]
        public long DynamicFormApprovedId { get; set; }
        public long? DynamicFormApprovalId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public bool? IsApproved { get; set; } = false;
        public string? ApprovedDescription { get; set; }
        [NotMapped]
        public long? DynamicFormId { get; set; }
        [NotMapped]
        public long? ApprovalUserId { get; set; }
    }
}
