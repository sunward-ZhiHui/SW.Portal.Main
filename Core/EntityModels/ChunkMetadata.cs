using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class ChunkMetadata
    {
        public int Index { get; set; }
        public int TotalCount { get; set; }
        public int FileSize { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public Guid FileGuid { get; set; }
    }
}
