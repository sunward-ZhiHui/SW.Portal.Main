using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_ProductionActivityReport
    {
        public long ProductionActivityAppLineID { get; set; }
        public string? ProdOrderNo { get; set; }
        public string? ProfileNo { get; set;}
        public string? TicketNo { get; set;}
        public string? LocationName { get; set;}
        public string? AddedDate { get; set;}
        public string? Process { get; set;}
        public string? Category { get; set;}
        public string? ActionList { get; set;}
        public string? Comment { get; set;}
        public string? Result { get; set;}
        public string? ActivityMaster { get; set;}
        public string? ActivityStatus { get; set;}
        public Guid? SessionID {  get; set;}
        public string? FilePath { get; set;}
        public List<string>? DocumentList { get; set; }
    }
    public class imgDocList
    {
        public string? FilePath { get; set; }
    }
}
