using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ApplicationMaster
    {
        [Key]
        public long ApplicationMasterId { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string ApplicationMasterName { get; set; }
        [Required(ErrorMessage = "Description is Required")]
        public string ApplicationMasterDescription { get; set; }
        public long? ApplicationMasterCodeId { get; set; }
        public bool? IsApplyProfile { get; set; }
        public bool? IsApplyFileProfile { get; set; }
    }
}
