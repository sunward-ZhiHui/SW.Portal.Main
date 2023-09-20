using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class UserGroupUser
    {
        [Key]
        public long UserGroupUserId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        [NotMapped]
        public string? UserGroupName { get; set;}
    }
}
