using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class view_QCAssignmentRM
    {
        public string? QCReferenceNo { get; set; }
        public string? JobDescription { get; set; }
        public string? Test { get; set; }
            
        public string? Person { get; set; }
        public string? Date { get; set; }
        public string? Entry_ID { get; set; }
        public string? SpecificTest { get; set; }
        public string? Company { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
    }
}
