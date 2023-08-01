using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class NavProductionInformation
    {
        [Key]
        public long NavProductionInformationId { get; set; }
        public long? ItemId { get; set; }
        [Required(ErrorMessage = "No Of BUOM In One Carton is Required")]
        public int? NoOfBUOMInOneCarton { get; set; }
    }
}
