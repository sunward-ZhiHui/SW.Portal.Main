using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailSearch
    {
        public string? BySubject { get; set; }
        public string? ByTag { get; set; }
        public IEnumerable<long> FromIds { get; set; }
        public string ByFrom { get; set; }
        public DateTime? FilterFrom { get; set; }
        public DateTime? FilterTo { get; set; }
        public string? MSearchText { get; set; }
        public long UserID { get; set; }
        public string? GroupTag { get; set; }
        public string? CategoryTag { get; set; }
        public string? ActionTag { get; set; }
        public string? Name { get; set; }
        public string? UserTag { get; set; }
        public bool UnArchive { get; set; }
    }
}
