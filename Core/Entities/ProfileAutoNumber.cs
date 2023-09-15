using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ProfileAutoNumber
    {
        [Key]
        public long ProfileAutoNumberId { get; set; }
        public long? ProfileId { get; set; }
        public long? CompanyId { get; set; }
        public long? DepartmentId { get; set; }
        public long? SectionId { get; set; }
        public long? SubSectionId { get; set; }
        public string LastNoUsed { get; set; }
        public long? ProfileYear { get; set; }
        public string ScreenId { get; set; }
        public long? ScreenAutoNumberId { get; set; }
    }
}
