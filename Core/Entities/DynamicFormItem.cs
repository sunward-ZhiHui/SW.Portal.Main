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
    public class DynamicFormItem : BaseEntity
    {
        [Key]
        public long DynamicFormItemID { get; set; }
        public string? AutoNumber { get; set; }
        [Required(ErrorMessage = "Company is Required")]
        public long? CompanyID { get; set; }
        [Required(ErrorMessage = "Transaction Type is Required")]
        public int? TransactionTypeID { get; set; }
        [Required(ErrorMessage = "Transaction Date is Required")]
        public DateTime? TransactionDate { get; set; }
        public string? Description { get; set; }
        [NotMapped]
        public string? TransactionTypeName { get; set; }
        [NotMapped]
        public string? CompanyName { get; set; }
        public string? Reason { get; set; }
        [Required(ErrorMessage = "Stock Group is Required")]
        public long? StockGroupAppParentID { get; set; }
        public long? DepartmentAppParentID { get; set; }
        public long? ItemGroupAppParentID { get; set; }
        public string? StockGroupAppParent { get; set; }
        public string? DepartmentAppParent { get; set; }
        public string? ItemGroupAppParent { get; set; }
    }
}
