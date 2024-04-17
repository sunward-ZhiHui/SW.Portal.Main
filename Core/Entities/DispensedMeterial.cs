using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DispensedMeterial : BaseEntity
    {
        [Key]

        public long? DispensedMeterialID { get; set; }
        public string MaterialName { get; set; }
        public string QCReference { get; set; }
        public string ProductionOrderNo { get; set; }
        public string Description { get; set; }
        public string BatchNo { get; set; }
        public string SubLotNo { get; set; }
        public string TareWeight { get; set; }
        public string ActualWeight { get; set; }
        public long UOM { get; set; }
        public string PrintLabel { get; set; }
    }
    
}
