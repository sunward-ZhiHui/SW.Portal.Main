using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ChangeControlForm :BaseEntity
    {
        [Key]
        public long ChangeControlFormID { get; set; }
        public string VersionNo { get; set; }
        public string CCNumber { get; set; }
        public string DocNo { get; set; }
    }
}
