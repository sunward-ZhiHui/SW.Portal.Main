using Core.Entities.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DynamicFormSectionAttrFormulaFunction
    {
        [Key]
        public long DynamicFormSectionAttrFormulaFunctionId { get; set; }
        public long? DynamicFormSectionAttributeId { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public long? DynamicFormSectionAttrFormulaMasterFuntionId { get; set; }
        [Required(ErrorMessage = "A value is required")]
        public decimal? AValue { get; set; }
        [Required(ErrorMessage = "B value is required")]
        [DynamicFormWorkFormFormulaValueCustomValidation]
        public decimal? BValue { get; set; }
        public bool IsBValueEnabled { get; set; } = false;
        [Required(ErrorMessage = "Color Code is required")]
        public string? ColorValue { get; set; }
        [NotMapped]
        public string? Type { get; set; }
        [NotMapped]
        public string? FormulaFunctionName { get; set; }
        public bool? IsCalculate { get; set; } = true;
        public string? FormulaTextBox { get; set; }
    }
    public class DynamicFormSectionAttrFormulaMasterFunction
    {
        [Key]
        public long DynamicFormSectionAttrFormulaMasterFunctionId { get; set; }
        public long? MasterID { get; set; }
        public string? Type { get; set; }
        public string? FormulaFunctionName { get; set; }
        public bool IsBValueEnabled { get; set; } = false;
    }
}
