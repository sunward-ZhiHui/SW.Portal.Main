using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class AttachmentModel : BaseModel
    {
        public long TaskAttachmentID { get; set; }
        public long? TaskMasterID { get; set; }
        public long? DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string Description { get; set; }
        public bool? IsVideoFile { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
    }
}
