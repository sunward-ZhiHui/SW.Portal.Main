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
    public class FileProfileTypeCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var datas = (DocumentDmsShare)validationContext.ObjectInstance;
            if (datas.IsExpiry == true && datas.ExpiryDate == null)
            {
                return new ValidationResult("Expiry Date is Required", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileProfileTypeNameCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public FileProfileTypeNameCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (FileProfileTypeModel)validationContext.ObjectInstance;
                string s = value.ToString().Trim();

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IFileprofileQueryRepository>();
                    var results = service.GeFileProfileTypeNameCheckValidation(s, datas.FileProfileTypeId);
                    if (results != null)
                    {
                        return new ValidationResult("Name already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
