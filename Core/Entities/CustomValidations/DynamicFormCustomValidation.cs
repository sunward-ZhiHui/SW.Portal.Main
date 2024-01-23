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
                var withoutSpecial = new string(s.Where(c => Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c)).ToArray());
                bool fHasSpace = s.Contains(" ");
                if (s != withoutSpecial || fHasSpace == true)
                {
                    return new ValidationResult("Special character,no white space not allowed", new[] { validationContext.MemberName });
                }
                else
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetService<IDynamicFormQueryRepository>();
                        var results = service.GetDynamicFormScreenNameCheckValidation(s, otherPropertyValue);
                        if (results != null)
                        {
                            return new ValidationResult("Screen Name already exits", new[] { validationContext.MemberName });
                        }
                    }
                }

            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormApprovalCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DynamicFormApprovalCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var dynamicFormApprovalId = validationContext.ObjectType.GetProperty("DynamicFormApprovalId");
                long dynamicFormApprovalID = (long)dynamicFormApprovalId.GetValue(validationContext.ObjectInstance, null);
                var DynamicFormId = validationContext.ObjectType.GetProperty("DynamicFormId");
                long dynamicFormID = (long)DynamicFormId.GetValue(validationContext.ObjectInstance, null);
                long? s = (long?)value;

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IDynamicFormQueryRepository>();
                    var results = service.GetDynamicFormApprovalUserCheckValidation(dynamicFormID, dynamicFormApprovalID, s);
                    if (results != null)
                    {
                        return new ValidationResult("User already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormApprovalIsUploadCustomValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var datas = (DynamicForm)validationContext.ObjectInstance;
            if (datas.IsUpload == true && datas.FileProfileTypeId == null)
            {
                return new ValidationResult("FileProfile is Required", new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class UserGroupNameCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public UserGroupNameCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var otherProperty = validationContext.ObjectType.GetProperty("UserGroupId");
                long otherPropertyValue = (long)otherProperty.GetValue(validationContext.ObjectInstance, null);
                string s = value.ToString();
                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IUserGroupQueryRepository>();
                    var results = service.GetUserGroupNameCheckValidation(s, otherPropertyValue);
                    if (results != null)
                    {
                        return new ValidationResult("User Group already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
