using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProductActivityCaseCategoryMultiple
    {
        [Key]
        public long ProductActivityCaseCategoryMultipleId { get; set; }
        public long? CategoryActionId { get; set; }
        public long? ProductActivityCaseId { get; set; }
    }
}
