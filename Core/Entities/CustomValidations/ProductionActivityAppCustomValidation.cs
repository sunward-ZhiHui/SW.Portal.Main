using Core.Entities.Views;
using Core.EntityModels;
using Core.Helpers;
using Core.Repositories.Query;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.Entities.CustomValidations
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ProductionActivityAppOtherOptionCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var datas = (ProductActivityAppModel)validationContext.ObjectInstance;
            if (datas.NavprodOrderLineId > 0)
            {
            }
            else
            {
                return new ValidationResult("Other Option is Required", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ProductionActivityAppProdOrderNoCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var datas = (ProductActivityAppModel)validationContext.ObjectInstance;
            if (datas.NavprodOrderLineId > 0 || string.IsNullOrEmpty(datas.OthersOptions))
            {
                var otherProperty = validationContext.ObjectType.GetProperty("OthersOptions");
                otherProperty.SetValue(validationContext.ObjectInstance, string.Empty);
               // datas.OthersOptions = string.Empty;
            }
            else
            {
                return new ValidationResult("Prod OrderNo is Required", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
}
