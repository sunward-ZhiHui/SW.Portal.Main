using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmployeeEmailInfoAuthority
    {
        [Key]
        public long EmployeeEmailInfoAuthorityID { get; set; }
        public long? EmployeeEmailInfoID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Purpose { get; set; }
    }
}
