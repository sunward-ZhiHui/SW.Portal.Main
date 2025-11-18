using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationRolePermission
    {
        public long RolePermissionID { get; set; }
        public long RoleID { get; set; }
        public long PermissionID { get; set; }
        [NotMapped]
        public string PermissionIDs { get; set; }
        [NotMapped]
        public DateTime AddedDate { get; set; }
        [NotMapped]
        public long? ModifiedByUserID { get; set; }
        [NotMapped]
        public DateTime? ModifiedDate { get; set; }
        [NotMapped]
        public long? AddedByUserID { get; set; }
        [NotMapped]
        public string? PermissionName {  get; set; }
        [NotMapped]
        public string? AddedBy {  get; set; }   
        [NotMapped]
        public string? ModifiedBy { get; set; }
        [NotMapped]
        public string? RolePermissionName {  get; set; }
    }
}
