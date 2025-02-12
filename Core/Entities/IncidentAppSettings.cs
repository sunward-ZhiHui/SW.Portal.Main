using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class IncidentAppSettings
    {
        [Key]
        public long IncidentAppSettingsID { get; set; }
        [Required]
        public long? SupportingDocFileProfileID { get; set; }
        [NotMapped]
        public string? Name { get; set; }
    }
}
