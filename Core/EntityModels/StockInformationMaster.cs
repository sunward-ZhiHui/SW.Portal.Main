using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class StockInformationMaster : BaseEntity
    {
        [Key]
        public long StockInformationMasterID { get; set; }
        [Required(ErrorMessage = "Item is Required")]
        public string? MasterName { get; set; }
        public long? CompanyID { get; set; }     
        public string? CompanyName { get; set; } 
        public long? CustomerID { get; set; }
        public string? CustomerName { get; set; } 
        public long? PlanningCategory { get; set; }
        public string? PlanningCategoryName { get; set; }
        public decimal? BelowMonth { get; set; }  
        public decimal? TopupMonth { get; set; }
        public string? PlanningCategoryDescName { get; set; }

    }
}
