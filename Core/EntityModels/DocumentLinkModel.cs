using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class DocumentLinkModel : BaseModel
    {
        public long DocumentLinkId { get; set; }
        public long? DocumentId { get; set; }
        public long? LinkDocumentId { get; set; }
        public string DocumentPath { get; set; }
        public string ContentType { get; set; }
        public long? FileProfieTypeId { get; set; }
        public long? FolderId { get; set; }
        public List<long?> DocumentIds { get; set; }
        public string DocumentName { get; set; }
        public string LinkDocumentName { get; set; }
        public string FullPath { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public byte?[] FileData { get; set; }
        public byte[] ImageData { get; set; }
        public int? FileIndex { get; set; }
        public long? FileSize { get; set; }
        public long? FileProfileTypeParentId { get; set; }
        public long? PathFileProfieTypeId { get; set; }
        public string WikiStatusType { get; set; }
        public Guid? MainSessionId { get; set; }
        public string FilePath { get; set; }
        public bool? IsNewPath { get; set; }
    }
}
