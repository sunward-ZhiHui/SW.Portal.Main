using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class QCTestRequirement:BaseEntity
    {
        public string? No { get; set; }
        public DateTime? Date { get; set; }
        public string? ItemName {  get; set; }  
        public string? BatchNo { get; set; }
        public string? Reference { get; set; }
        public int VersionNo { get; set; }
        public string? PurposeOfTest { get; set; }

    }

    public class QCTestRequirementChild
    {
        public string? Process { get; set; }
        public string? TestName { get; set; }
        public string? QCReferenceNo { get; set; }
        public string? Equipment { get; set; }
        public DateTime? TestStartDateBy { get; set; }
        public string? RecordComplrtion { get; set; }
        public string? QCReviwedBy { get; set; }

    }
}
