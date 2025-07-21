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
    public class SPCDataTrending : BaseEntity 
    {
        [Key]
        public long ID { get; set; }
        [DisplayName("QC RefNo")]
        [Required(ErrorMessage = "QC RefNo.")]
        public string? QCRefNo { get; set; }
        [DisplayName("Assay")]
        public decimal Assay { get; set; }
    }
}
