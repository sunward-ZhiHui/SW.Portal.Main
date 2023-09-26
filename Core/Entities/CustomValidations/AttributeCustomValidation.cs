using Core.Entities.Views;
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
    public class AttributeCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string s = value.ToString();
                var regexItem = new Regex(@"^[a-zA-Z0-9'' ']+$");
                var withoutSpecial = new string(s.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)).ToArray());
                bool fHasSpace = s.Contains(" ");
                if (s != withoutSpecial || fHasSpace == true)
                {
                    return new ValidationResult("Special character and no white space should not be entered", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
