using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentUserRole
    {
        [Key]
        public long DocumentUserRoleId { get; set; }
        public long? UserId { get; set; }
        public long? RoleId { get; set; }
        public long? UserGroupId { get; set; }
        public long? DocumentId { get; set; }
        public long? FolderId { get; set; }
        public bool? IsFolderLevel { get; set; }
        public long? ReferencedGroupId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? LevelId { get; set; }

    }
}
