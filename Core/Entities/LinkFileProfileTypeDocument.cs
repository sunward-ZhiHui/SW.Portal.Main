using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class LinkFileProfileTypeDocument
    {
        [Key]
        public long LinkFileProfileTypeDocumentId { get; set; }
        public Guid? TransactionSessionId { get; set; }
        public long? DocumentId { get; set; }
        public long? FileProfileTypeId { get; set; }
        public long? FolderId { get; set; }
        public long? AddedByUserId { get; set; }
        public DateTime? AddedDate { get; set; }
        public string Description { get; set; }
        public bool? IsWiki { get; set; }
        public bool? IsWikiDraft { get; set; }
    }
}
