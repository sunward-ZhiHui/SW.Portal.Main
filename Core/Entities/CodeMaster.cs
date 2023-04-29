using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CodeMaster
    {
        [Key]   
        public int CodeId { get; set; }
        public string? CodeType { get; set; }
        public string? CodeValue { get; set; }
        public string? CodeDescription { get; set; }
        public long CodeMasterId { get; set; }
    }
}
