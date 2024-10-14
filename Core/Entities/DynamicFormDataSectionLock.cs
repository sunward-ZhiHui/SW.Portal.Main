using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormDataSectionLock
    {
        public long DynamicFormDataSectionLockId { get; set; }
        public long? DynamicFormDataId { get; set; }
        public long? DynamicFormSectionId { get; set; }
        public bool? IsLocked { get; set; } = false;
        public long? LockedUserId { get; set; }
        public string? LockedUser { get; set; }
        public string? ProfileNo { get; set; }
        public string? Name { get; set; }
        public string? SectionName { get; set; }
        public Guid? FormSessionId { get; set; }
        public Guid? FormDataSessionId { get; set; }
    }
}
