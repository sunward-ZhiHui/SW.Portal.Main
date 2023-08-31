using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_GetCCFImplementation
    {
        public long ApplicationMasterID { get; set; }
        public long ApplicationMasterDetailID { get; set; }
        public string Value { get; set; }   
        public string description { get; set; }
        public long ClassOFDocumentID { get; set; }
        public long CCFDImplementationDetailsID { get; set; }
        public long CCFDImplementationID { get; set; }
        public bool? IsRequired { get; set; }
        public long? ResponsibiltyTo { get; set; }
        public long? DoneBy { get; set; }
        public DateTime? DoneByDate { get; set; }
        public string ResponsibilityName { get; set; }
        public string DoneByName { get; set; }
        public Guid SessionID { get; set; }
    }
}
