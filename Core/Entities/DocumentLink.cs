using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DocumentLink
    {
        [Key]
        public long DocumentLinkId { get; set; }
        public long? DocumentId { get; set; }
        public long? LinkDocumentId { get; set; }
        public string DocumentPath { get; set; }
        public long? FileProfieTypeId { get; set; }
        public long? FolderId { get; set; }
        public int? StatusCodeId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public long? ModifiedByUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
