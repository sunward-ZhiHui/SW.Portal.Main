using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_ApplicationMasterDetail
    {
        public long ApplicationMasterDetailID { get; set; }
        public long? ApplicationMasterID { get; set; }
        public string? Description { get; set; }
        public string? ApplicationMasterName { get; set; }
        public string? Value { get; set; }
        public long? ApplicationMasterCodeID { get; set; }
    }
}
