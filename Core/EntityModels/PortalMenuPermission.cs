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
        public string PermissionName { get; set; }
        public string Icon { get; set; }
        public string ScreenID { get; set; }
        public List<PortalMenuModel> Items { get; set; }
        public long? ParentID { get; set; }
        public long? PermissionID { get; set; }
        public bool IsPermissionURL { get; set; }
        public Guid UniqueSessionID { get; set; }
        public string? PermissionURL { get; set; }
    }
    public class BreadcrumbModel
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public bool Selected { get; set; }
        public long? PermissionID { get; set; }
        public int Removed { get; set; } = 0;

    }
    public class PortalPermissionModel
    {
        public long? PermissionID { get; set; }
        public long? ParentID { get; set; }
        public string PermissionCode { get; set; }
        public string ScreenID { get; set; }
    }
    public class PortalMenuMultipleModel
    {
        public List<PortalMenuModel> PortalMenuModel { get; set; }
        public PortalMenuModel DashboardItem { get; set; }
    }
}

