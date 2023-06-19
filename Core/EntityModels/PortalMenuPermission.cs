using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class PortalMenuPermission
    {
        public List<PortalMenuModel> PortalMenuModels { get; set; }
        public List<PortalPermissionModel> PortalPermissionModels { get; set; }
    }
    public class PortalMenuModel
    {
        public string Title { get; set; }
        public string Header { get; set; }
        public string Group { get; set; }
        public string MenuOrder { get; set; }
        public string Component { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public string ScreenID { get; set; }
        public List<PortalMenuModel> Items { get; set; }
        public long? ParentID { get; set; }
        public long? PermissionID { get; set; }
    }

    public class PortalPermissionModel
    {
        public long? PermissionID { get; set; }
        public long? ParentID { get; set; }
        public string PermissionCode { get; set; }
        public string ScreenID { get; set; }
    }
}

