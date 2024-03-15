using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public decimal FullCartonQty { get; set; }
        public decimal LooseCartonQty { get; set; }
        public string PalletNo { get; set; }
        [NotMapped]
        public int TotalFullCartonQty { get; set;}
        [NotMapped]
        public int TotalLooseCartonQty { get; set; }
        [NotMapped]
        public int CountFullCartonQty { get; set; }
        [NotMapped]
        public int CountLooseCartonQty { get; set; }

    }
}
