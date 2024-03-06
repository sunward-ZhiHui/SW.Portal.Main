using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class FbOutputCartons:BaseEntity
    {
        [Key]
        public long FbOutputCartonID { get; set; }
        public string? LocationName { get; set; }
        public string ProductionOrderNo { get; set; }
        public string? CartonNo { get; set; }
        public string? ItemNo { get; set; }
        public string? Description { get; set; }
        public string? BatchNo { get; set; }
        public bool? FullCarton { get; set; }
        public string FullCartonQty { get; set; }
        public string LooseCartonQty { get; set; }
        public string PalletNo { get; set; }
        
    }
}
