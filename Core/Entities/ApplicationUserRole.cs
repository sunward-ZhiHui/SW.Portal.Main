using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationUserRole
    {
        [Key]
        public long UserRoleId { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
    }
}
