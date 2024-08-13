using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormApprovedChanged
    {
        public long DynamicFormApprovedChangedId { get; set; }
        public long? DynamicFormApprovedID { get; set; }
        [Required(ErrorMessage = "User Name is Required")]
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public bool? IsApprovedStatus { get; set; }
    }
}
