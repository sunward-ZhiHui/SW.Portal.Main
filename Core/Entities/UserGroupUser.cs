using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserGroupUser
    {
        public long UserGroupUserId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
    }
}
