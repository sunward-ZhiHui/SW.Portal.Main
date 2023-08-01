using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class ReportDocuments
    {
        public long ReportDocumentID { get; set; }
        public string FileName { get; set;}
        public string ContentType { get; set;}
        public int FileSize { get; set;}
        public Guid? SessionId { get; set; }
        public string FilePath { get; set;}
    }
}
