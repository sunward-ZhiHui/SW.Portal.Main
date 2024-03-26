using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class RequestEmail
    {       
        public long? ToIds  { get; set; }        
        public string? CcIds { get; set; }
    }
}
