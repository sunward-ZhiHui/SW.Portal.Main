using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationRolePermission
    {
        public long RolePermissionID { get; set; }
        public long RoleID { get; set;}
        public long PermissionID { get; set; }
    }
}
