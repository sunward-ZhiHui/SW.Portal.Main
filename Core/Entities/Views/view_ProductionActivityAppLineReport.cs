using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class view_ProductionActivityAppLineReport
    {
        public DateTime Date { get; set; }
        public long CompanyId { get; set; }
        public string DisciplineType { get; set; }
        public string Grouping { get; set; }
        public string Category { get; set; }
        public string Action { get; set; }
        public string Comment { get; set; }
        public string Result { get; set; }
        public string Status { get; set; }
        public string ReadBy { get; set; }
        public string Name { get; set; }
        public string ModifyBy { get; set; }
        public string Info { get; set; }
        public long ProductionActivityAppLineID { get; set; }
        public Guid SessionID { get; set; }
        
       

    }
}
