using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewProductionEntry
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
        public string ProductionActionName { get; set; }
        public byte[] FrontPhoto { get; set; }
        public byte[] BackPhoto { get; set; }
        public int? ProductionLineNo { get; set; }
        public string BatchNo { get; set; }
        public string Deascription { get; set; }
        public long? PostedUserID { get; set; }
        public int? ProcessNo { get; set; }
        public string RePlanRefNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
       
        public string AddedBy { get; set; }
       
      
       

    }
}
