using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class HeaderPropertyMapping
    {
        [Key]
        public long Id;
        public string? ExcelHeader { get; set; }
        public string? DocumentProperty { get; set; }
    }
}
