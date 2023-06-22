using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductionEntry:BaseEntity
    {
        public long ProductionEntryID { get; set; }
        public long? ProductionActionID { get; set; }
        public string ProductionOrderNo { get; set; }
        public string RoomStatus { get; set; }
        public string RoomStatus1 { get; set; }
        public List<long> ProdTaskIds { get; set; }
        public int? NumberOfWorker { get; set; }
        public string LocationName { get; set; }
        public string ItemName { get; set; }
        public long? LocationID { get; set; }
         public int ProdLineNo { get; set; }
        public string BatchNo { get; set; }
        public string Deascription { get; set; }
     
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
      
        
    }
}
