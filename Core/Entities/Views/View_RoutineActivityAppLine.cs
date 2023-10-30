using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class View_RoutineActivityAppLineReport
    {
        public DateTime Date { get; set; }
        //[Required(ErrorMessage = "Please Select Company.")]
        public long CompanyId { get; set; }
        public string DisciplineType { get; set; }
        public string Grouping { get; set; }
        public string Category { get; set; }
        public string Action { get; set; }
        public string Location { get; set; }
        public string Routine { get; set; }
        public string RoutineMaster { get; set; }
        public string Comment { get; set; }
        public string Result { get; set; }
        public string RoutineStatus { get; set; }
        public string ReadBy { get; set; }
        public string Name { get; set; }
        public string ModifyBy { get; set; }
        public string Info { get; set; }
        public long ProductionActivityRoutineAppLineId { get; set; }
        public Guid SessionID { get; set; }
        public string CommentImageType { get; set; }
        public byte[] CommentImage { get; set; }

    }
}
