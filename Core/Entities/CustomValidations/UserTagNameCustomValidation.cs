using Core.Repositories.Query;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.CustomValidations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserTagNameCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public UserTagNameCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                //var otherProperty = validationContext.ObjectType.GetProperty("UserTag");
                //string otherPropertyValue =(string) otherProperty.GetValue(validationContext.ObjectInstance, null);
                string s = value.ToString().Trim();
                  var usertagname = s.ToLower();
                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IEmailTopicsQueryRepository>();
                    var results = service.GetUserAsync(usertagname);
                    if (results != null)
                    {
                        return new ValidationResult("UserTag Name already exits", new[] { validationContext.MemberName });
                    }
                }

            }
            return ValidationResult.Success;
        }
    
    }
}
