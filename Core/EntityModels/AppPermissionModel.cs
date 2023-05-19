using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class AppPermissionModel
    {
        public long AppPermissionID { get; set; }
        public long? AppID { get; set; }
        public long? ParentID { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public List<AppPermissionModel> Children { get; set; } = new List<AppPermissionModel>();
    }

}
