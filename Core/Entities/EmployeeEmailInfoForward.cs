using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeEmailInfoForward
    {
        [Key]
        public long EmployeeEmailInfoForwardID { get; set; }
        public long? EmployeeEmailInfoID { get; set; }
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }
    }
}
