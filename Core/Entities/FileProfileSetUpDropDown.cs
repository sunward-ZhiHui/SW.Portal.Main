using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public partial class FileProfileSetUpDropDown
    {
        [Key]
        public long FileProfileSetUpDropDownId { get; set; }
        public long? FileProfileSetupFormId { get; set; }
        public string DisplayValue { get; set; }
    }
}
