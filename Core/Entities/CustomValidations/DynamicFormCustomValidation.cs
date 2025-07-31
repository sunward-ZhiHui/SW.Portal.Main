using Core.Entities.Views;
using Core.Helpers;
using Core.Repositories.Query;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using org.matheval;
using Expression = org.matheval.Expression;
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
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormSectionAttributeCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DynamicFormSectionAttributeCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DynamicFormSectionAttribute)validationContext.ObjectInstance;

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IDynamicFormQueryRepository>();
                    var results = service.GetDynamicFormSectionAttributeCheckValidation(datas.SelectDynamicFormId, datas.DynamicFormSectionAttributeId, datas.AttributeId);
                    if (results != null)
                    {
                        return new ValidationResult("Gird form already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFlowSequenceNoCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DynamicFormWorkFlowSequenceNoCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DynamicFormWorkFlow)validationContext.ObjectInstance;

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IDynamicFormQueryRepository>();
                    var results = service.GetDynamicFormWorkFlowSequenceNoExitsCheckValidation(datas.DynamicFormId, datas.DynamicFormWorkFlowId, datas.SequenceNo);
                    if (results != null)
                    {
                        return new ValidationResult("SequenceNo already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFormFlowSequenceNoCustomValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DynamicFormWorkFormFlowSequenceNoCustomValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DynamicFormWorkFlowForm)validationContext.ObjectInstance;

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IDynamicFormQueryRepository>();
                    var results = service.GetDynamicFormDataWorkFlowSequenceNoExitsCheckValidation(datas.DynamicFormDataId, datas.DynamicFormWorkFlowId, datas.SequenceNo);
                    if (results != null)
                    {
                        return new ValidationResult("SequenceNo already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFlowSectionNameCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DynamicFormWorkFlow)validationContext.ObjectInstance;
                if (datas.SelectDynamicFormSectionIDs == null)
                {
                    return new ValidationResult("Section Name is Required", new[] { validationContext.MemberName });
                }
                else
                {
                    if (datas.SelectDynamicFormSectionIDs.Count() > 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Section Name is Required", new[] { validationContext.MemberName });
                    }
                }
            }
            else
            {
                return new ValidationResult("Section Name is Required", new[] { validationContext.MemberName });
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFlowMultipleUsersCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var datas = (DynamicFormWorkFlow)validationContext.ObjectInstance;
            if (value != null)
            {
                if (datas.IsAnomalyStatus == false && datas.IsMultipleUser==true)
                {
                    if (datas.SelectUserIDs == null)
                    {
                        return new ValidationResult("User Name is Required", new[] { validationContext.MemberName });
                    }
                    else
                    {
                        if (datas.SelectUserIDs.Count() > 0)
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("User Name is Required", new[] { validationContext.MemberName });
                        }
                    }

                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("User Name is Required", new[] { validationContext.MemberName });
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFlowFormMultipleUsersCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var datas = (DynamicFormWorkFlowForm)validationContext.ObjectInstance;
            if (value != null)
            {
                if (datas.IsAnomalyStatus == false && datas.IsMultipleUser == true)
                {
                    if (datas.SelectUserIDs == null)
                    {
                        return new ValidationResult("User Name is Required", new[] { validationContext.MemberName });
                    }
                    else
                    {
                        if (datas.SelectUserIDs.Count() > 0)
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("User Name is Required", new[] { validationContext.MemberName });
                        }
                    }

                }
                else
                {
                    return ValidationResult.Success;
                }
            }
            else
            {
                return new ValidationResult("User Name is Required", new[] { validationContext.MemberName });
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFormFlowSectionCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DynamicFormWorkFlowForm)validationContext.ObjectInstance;
                if (datas.SelectDynamicFormSectionIDs == null)
                {
                    return new ValidationResult("Section Name is Required", new[] { validationContext.MemberName });
                }
                else
                {
                    if (datas.SelectDynamicFormSectionIDs.Count() > 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Section Name is Required", new[] { validationContext.MemberName });
                    }
                }
            }
            else
            {
                return new ValidationResult("Section Name is Required", new[] { validationContext.MemberName });
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormSectionAttributeFormulaCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DynamicFormSectionAttribute)validationContext.ObjectInstance;
                if (!string.IsNullOrEmpty(datas.FormulaTextBox))
                {
                    string str = datas.FormulaTextBox;
                    if (!string.IsNullOrEmpty(datas.FormulaTextBox))
                    {
                        for (int i = 20001; i-- > 0;)
                        {
                            str = str.Replace("^" + i, "");
                            str = str.Replace("-" + i, "");
                            str = str.Replace("/" + i, "");
                        }
                    }
                    var values = str.Split("$");
                    var formulaText = datas.FormulaTextBox;
                    List<string> list = new List<string>();
                    if (values.Length > 0)
                    {
                        foreach (var item in values)
                        {
                            if (!string.IsNullOrEmpty(item))
                            {
                                list.Add(Regex.Replace(item, @"[^0-9a-zA-Z_]+", ""));
                                //list.Add(Regex.Match(item, @"\d+").Value);
                            }
                        }
                        list = list.Distinct().ToList();
                        if (list.Count > 0)
                        {
                            int i = 65;
                            list.ForEach(s =>
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    string strAlpha = ((char)i).ToString();
                                    formulaText = formulaText.Replace("$" + s, strAlpha.ToLower());
                                    i++;
                                }
                            });
                            Expression expressions = new Expression(formulaText);
                            int j = 65;
                            list.ForEach(s =>
                            {
                                if (!string.IsNullOrEmpty(s))
                                {
                                    string strAlpha = ((char)j).ToString();
                                    expressions.Bind(strAlpha.ToLower(), 10);
                                    j++;
                                }
                            });
                            List<String> errors = expressions.GetError();

                            if (errors.Count == 0)
                            {
                                object valuess = expressions.Eval<object>();
                                var val = valuess;
                            }
                            else
                            {
                                return new ValidationResult(errors[0], new[] { validationContext.MemberName });
                            }
                        }
                    }
                }

            }
            return ValidationResult.Success;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SelectUserGropupUserCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (UserGroup)validationContext.ObjectInstance;
                if (datas.SelectUserIDs == null)
                {
                    return new ValidationResult("User is Required", new[] { validationContext.MemberName });
                }
                else
                {
                    if (datas.SelectUserIDs.Count() > 0)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("User is Required", new[] { validationContext.MemberName });
                    }
                }
            }
            else
            {
                return new ValidationResult("User is Required", new[] { validationContext.MemberName });
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DynamicFormWorkFormFormulaValueCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var datas = (DynamicFormSectionAttrFormulaFunction)validationContext.ObjectInstance;
            if (datas.IsBValueEnabled == true)
            {
                if (datas.BValue != null)
                {
                    if (datas.BValue > datas.AValue)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("B Value is Smaller than A Value. It must Greater", new[] { validationContext.MemberName });
                    }
                }
                else
                {
                    return new ValidationResult("This field is Required", new[] { validationContext.MemberName });
                }

            }
            else
            {
                return ValidationResult.Success;
            }

        }
    }
}
