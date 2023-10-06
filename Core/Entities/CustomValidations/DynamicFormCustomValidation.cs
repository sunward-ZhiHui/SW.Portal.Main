using Core.Entities.Views;
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
    public class DynamicFormCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DynamicFormCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectType.GetProperty("ID");
                long otherPropertyValue = (long)otherProperty.GetValue(validationContext.ObjectInstance, null);
                string s = value.ToString();
                var regexItem = new Regex(@"^[a-zA-Z'' ']+$");
                var withoutSpecial = new string(s.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c)).ToArray());
                bool fHasSpace = s.Contains(" ");
                if (s != withoutSpecial || fHasSpace == true)
                {
                    return new ValidationResult("Special character,Numbers and no white space not allowed", new[] { validationContext.MemberName });
                }
                else
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetService<IDynamicFormQueryRepository>();
                        var results = service.GetDynamicFormScreenNameCheckValidation(s, otherPropertyValue);
                        if(results!=null)
                        {
                            return new ValidationResult("Screen Name already exits", new[] { validationContext.MemberName });
                        }
                    }
                }
                
            }
            return ValidationResult.Success;
        }
    }

}
