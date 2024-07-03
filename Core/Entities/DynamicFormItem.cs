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
    public class DynamicFormItem:BaseEntity
    {
        [Key]
        public long DynamicFormItemID { get; set; }
          public string AutoNumber { get; set; }
        public long? CompanyID { get; set; }
      public int TransactionTypeID { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string  Description { get; set;}
        [NotMapped] 
      public string TransactionTypeName { get; set; }
        [NotMapped]
        public string CompanyName { get; set; }
    }
}
