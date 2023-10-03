using Core.Entities.Views;
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
    public class AttributeCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string s = value.ToString().Trim();
                var regexItem = new Regex(@"^[a-zA-Z'' ']+$");
                var withoutSpecial = new string(s.Where(c => Char.IsLetter(c) || Char.IsWhiteSpace(c)).ToArray());
                bool fHasSpace = s.Contains(" ");
                if (s != withoutSpecial || fHasSpace == true)
                {
                    return new ValidationResult("Special character,Numbers and no white space not allowed", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        public class AttributeValueCustomValidation : ValidationAttribute
        {
            private IServiceProvider serviceProvider;

            public AttributeValueCustomValidation()
            {
                serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
            }
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value != null)
                {
                    var attributeDetailID = validationContext.ObjectType.GetProperty("AttributeDetailID");
                    long attributeDetailId = (long)attributeDetailID.GetValue(validationContext.ObjectInstance, null);
                    var attributeID = validationContext.ObjectType.GetProperty("AttributeID");
                    long attributeId = (long)attributeID.GetValue(validationContext.ObjectInstance, null);
                    string s = value.ToString().Trim();

                    using (var scope = serviceProvider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetService<IAttributeDetailsQueryRepository>();
                        var results = service.AttributeDetailsValueCheckValidation(s, attributeId, attributeDetailId);
                        if (results != null)
                        {
                            return new ValidationResult("Value already exits", new[] { validationContext.MemberName });
                        }
                    }
                }
                return ValidationResult.Success;
            }
        }
    }
}
