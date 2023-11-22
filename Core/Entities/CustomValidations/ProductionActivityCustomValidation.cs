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
    public class ProductionActivityToEmailCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var datas = (ActivityEmailTopicsModel)validationContext.ObjectInstance;
            if (datas.ToId == null)
            {
                return new ValidationResult("To User is Required", new[] { validationContext.MemberName });
            }
            else
            {
                if (datas.ToId.Count() == 0)
                {
                    return new ValidationResult("To User is Required", new[] { validationContext.MemberName });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ProductionActivityCcEmailCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var datas = (ActivityEmailTopicsModel)validationContext.ObjectInstance;
            if (datas.CcId == null)
            {
                return new ValidationResult("CC User is Required", new[] { validationContext.MemberName });
            }
            else
            {
                if (datas.CcId.Count() == 0)
                {
                    return new ValidationResult("CC User is Required", new[] { validationContext.MemberName });
                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            return ValidationResult.Success;
        }
    }

}
