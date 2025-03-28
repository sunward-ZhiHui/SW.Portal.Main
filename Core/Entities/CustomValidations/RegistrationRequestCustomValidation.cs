using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.CustomValidations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class RegistrationRequestCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (RegistrationRequestDepartmentEmailCreate)validationContext.ObjectInstance;
                if (datas.ToIds == null)
                {
                    return new ValidationResult("To User is Required", new[] { validationContext.MemberName });
                }
                else
                {
                    if (datas.ToIds.Count() > 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("To User is Required", new[] { validationContext.MemberName });
                    }
                }
            }
            else
            {
                return new ValidationResult("To User is Required", new[] { validationContext.MemberName });
            }
        }
    }
}
