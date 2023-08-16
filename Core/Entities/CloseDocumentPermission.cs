using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CloseDocumentPermission
    {
        [Key]
        public long CloseDocumentPermissionId { get; set; }
        public long? UserId { get; set; }
        public long? UserGroupId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public bool? IsCloseDocumentPermission { get; set; }
    }
}
