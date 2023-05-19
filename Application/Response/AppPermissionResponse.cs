using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Response
{
    public class AppPermissionResponse
    {
        public long AppPermissionID { get; set; }
        public long? AppID { get; set; }
        public long? ParentID { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public List<AppPermissionResponse> Children { get; set; } = new List<AppPermissionResponse>();
    }
}
