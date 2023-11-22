using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public partial class ProductActivityCaseActionMultiple
    {
        [Key]
        public long ProductActivityCaseActionMultipleId { get; set; }
        public long? ProductActivityCaseId { get; set; }
        public long? ActionId { get; set; }
    }
}
