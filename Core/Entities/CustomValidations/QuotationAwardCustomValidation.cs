using Core.Entities.Views;
using Core.Repositories.Query;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
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
    public class QuotationAwardCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string s = value.ToString().Trim();
                var withoutSpecial = new string(s.Where(c => Char.IsDigit(c) || Char.IsWhiteSpace(c)).ToArray());
                bool fHasSpace = s.Contains(" ");
                if (s != withoutSpecial || fHasSpace == true)
                {
                    return new ValidationResult("Only Number Allowed", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class QuotationAwardDifferentPriceCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string s = value.ToString().Trim();
                if (!string.IsNullOrEmpty(s))
                {
                    if (decimal.TryParse(s, out _))
                    {
                        // valid decimal 
                    }
                    else
                    {
                        return new ValidationResult("Only Number Allowed", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
