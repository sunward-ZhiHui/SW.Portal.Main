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
    public class ReportDocuments
    {
        [Key]
        public long ReportDocumentID { get; set; }
        
        public string FileName { get; set;}
        public string ContentType { get; set;}
        public int FileSize { get; set;}
        public Guid? SessionId { get; set; }
        public string FilePath { get; set;}
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsLatest { get; set; }
        [NotMapped]
        public byte[]? FileData { get; set; }
       
    }
}
