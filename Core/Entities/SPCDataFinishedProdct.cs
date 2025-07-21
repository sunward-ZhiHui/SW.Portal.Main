using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class SPCDataFinishedProdct : BaseEntity
    {
        [Key]
        public long ID { get; set; }
        [DisplayName("Batch No")]
        [Required(ErrorMessage = "Please enter the Batch No.")]
        public string? BatchNo { get; set; }
        [DisplayName("Trend of Final Product Analysis")]
        [Required(ErrorMessage = "Please enter the Value.")]
        public decimal Value { get; set; } // Assay or Hardness
        [DisplayName("Remarks")]
        public string? Remarks { get; set; }
    }
}
