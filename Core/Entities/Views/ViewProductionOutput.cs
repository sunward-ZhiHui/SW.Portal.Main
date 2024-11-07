using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Views
{
    public class ViewProductionOutput
    {
        public long ProductionOutputId { get; set; }
        public string LocationName { get; set; }
        public long? ItemId { get; set; }
        public string ItemNo { get; set; }
        public long? ProductionEntryId { get; set; }
        public string ProductionOrderNo { get; set; }
        public string SubLotNo { get; set; }
        public string LotNo { get; set; }
        public string DrumNo { get; set; }
        public string Description { get; set; }
        public string BatchNo { get; set; }
        public string QCRefNo { get; set; }
        public decimal? OutputQty { get; set; }
        public decimal? NetWeight { get; set; }
        public string Buom { get; set; }
        public bool? IsLotComplete { get; set; }
        public bool? IsPostedToNAV { get; set; }
        public bool? IsProdutionOrderComplete { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string OutputDate { get; set; }
        public string Time { get; set; }
        public string GrossWeight { get; set; }
        public string LocationDescription { get; set; }
    }
}
