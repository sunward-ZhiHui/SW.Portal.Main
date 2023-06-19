using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewConsumptionLines
    {
        public long ConsumptionLineID { get; set; }
        public long? ConsumptionEntryID { get; set; }
        public string ItemNo { get; set; }
        public string Description { get; set; }
        public string LotNo { get; set; }
        public string SubLotNo { get; set; }
        public string QCRefNo { get; set; }
        public string BatchNo { get; set; }
        public bool? IsFullConsume { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? AvailableQuantity { get; set; }
        public string UOM { get; set; }
        public decimal? BaseQuantity { get; set; }
        public string BaseUOM { get; set; }
        public int ProdLineNo { get; set; }
        public bool? PostedtoNav { get; set; }
        public int ProdComLineNo { get; set; }
        public string AddedDate { get; set; }
        public string AddedBy { get; set; }
        public int LineNo { get; set; }
        public string ProductionOrderNo { get; set; }
        public int? DrumNo { get; set; }
    }
}
