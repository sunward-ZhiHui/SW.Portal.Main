using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Command.ApplicationUserRoles
{
    public class ApplicationUserRoleCommand
    {
        public long UserRoleId { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }
        public ApplicationUserRoleCommand() { }
    }
}
