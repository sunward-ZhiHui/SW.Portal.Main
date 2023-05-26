using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class SpUserPermission
    {
        public long PermissionID { get; set; }
        public string PermissionName { get; set; }
        public long? ParentID { get; set; }
        public byte PermissionLevel { get; set; }
        public string PermissionURL { get; set; }
        public string PermissionGroup { get; set; }
        public string PermissionOrder { get; set; }
        public string PermissionCode { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public bool IsDisplay { get; set; }
        public bool? IsMobile { get; set; }
        public bool? IsNewPortal { get; set; }
        public string Icon { get; set; }
        public string MenuId { get; set; }
        public string Name { get; set; }
        public string Component { get; set; }
        public string ScreenID { get; set; }
        public bool? IsCmsApp { get;set; }
    }
}
