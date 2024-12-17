using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormDataAssignUser
    {
        [Key]
        public long DynamicFormDataAssignUserId { get; set; }
        public long? DynamicFormId { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        [NotMapped]
        public IEnumerable<long?> SelectUserIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectUserGroupIDs { get; set; } = new List<long?>();
        [NotMapped]
        public IEnumerable<long?> SelectLevelMasterIDs { get; set; } = new List<long?>();
        public Guid? SessionId { get; set; }
    }
}
