using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProdOrderMultiple
    {
        public long ProdOrderMultipleID { get; set; }   
        public long ProductionActivityRoutineAppLineID { get; set; }
        public string FPDDMultiple { get; set; }
        public string ProcessDDMultiple { get; set; }
        public string RawMaterialDDMultiple { get; set; }
        public string PackingMaterialDDMultiple { get; set; }
        public long? ProductNameMultiple { get; set; }
        public string Type { get; set; }
    }
}
