using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationUser : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        public string? LoginID { get; set; }
        public string? Password { get; set; }
        public string? LoginPassword { get; set; }
        public string UserName { get; set; }    
        public string UserCode { get; set; }
    }
}
