using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ProductGroupingNavDifferenceModel : BaseModel
    {
        public long ProductGroupingNavDifferenceId { get; set; }
        public long? ProductGroupingNavId { get; set; }
        [Required(ErrorMessage = "Type Of Difference is Required")]
        public long? TypeOfDifferenceId { get; set; }
        public string? TypeOfDifference { get; set; }
        public string? DifferenceInfo { get; set; }
    }
}
