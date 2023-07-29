using Core.Entities.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.CustomValidations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PlantsValidaion : ValidationAttribute
    {
        public string Description { get; set; }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {

               /* var otherProperty = validationContext.ObjectType.GetProperty(Description);
                var otherPropertyValue = otherProperty.GetValue(validationContext.ObjectInstance, null);
                var values = result.PlantCode;

                if (Convert.ToString(value) == values)
                {
                    return new ValidationResult("Plant Code is Exits", new[] { validationContext.MemberName });
                }*/
            }
            return ValidationResult.Success;
        }
    }

    public class DescriptionValidaion : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                if (Convert.ToString(value) == "sdss")
                {
                    return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
