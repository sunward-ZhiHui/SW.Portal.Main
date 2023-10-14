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
    public class DocumentRoleCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DocumentRoleCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectType.GetProperty("DocumentRoleId");
                long otherPropertyValue = (long)otherProperty.GetValue(validationContext.ObjectInstance, null);
                string s = value.ToString().Trim();
                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IFileprofileQueryRepository>();
                    var results = service.GetDocumentRoleNameCheckValidation(s, otherPropertyValue);
                    if (results != null)
                    {
                        return new ValidationResult("Role Name already exits", new[] { validationContext.MemberName });
                    }
                }

            }
            return ValidationResult.Success;
        }
    }

}
