using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class SyncResult
    {
        public int ReadCount { get; set; }
        public int InsertedCount { get; set; }
        public int SkippedCount { get; set; }
    }
}
