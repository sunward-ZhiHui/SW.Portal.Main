using Core.Entities.Views;
using Core.EntityModels;
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
    public class DocumentProfileNoSeriesValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DocumentProfileNoSeriesValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DocumentProfileNoSeriesModel)validationContext.ObjectInstance;

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IDocumentProfileNoSeriesQueryRepository>();
                    var results = service.GetProfileNameCheckValidation(datas.Name, datas.ProfileID);
                    if (results != null)
                    {
                        return new ValidationResult("Profile Name already exits", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DocumentProfileNoSeriesAbbreviationValidation : ValidationAttribute
    {
        private IServiceProvider serviceProvider;

        public DocumentProfileNoSeriesAbbreviationValidation()
        {
            serviceProvider = AppDependencyResolver.Current.GetService<IServiceProvider>();
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DocumentProfileNoSeriesModel)validationContext.ObjectInstance;

                using (var scope = serviceProvider.CreateScope())
                {
                    var service = scope.ServiceProvider.GetService<IDocumentProfileNoSeriesQueryRepository>();
                    var results = service.GetAbbreviationCheckValidation(datas.Name, datas.Abbreviation, datas.ProfileID);
                    if (results != null)
                    {
                        return new ValidationResult("This Profile Name or Profile Appreviation already exist!", new[] { validationContext.MemberName });
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class EstablishNoSeriesCodeIDCustomValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null)
            {
                var datas = (DocumentProfileNoSeriesModel)validationContext.ObjectInstance;
                if (datas.IsNoSerieswithOrganisationinfo == true)
                {
                    if (datas.EstablishNoSeriesCodeIDs == null)
                    {
                        return new ValidationResult("Establish No Series Code is Required", new[] { validationContext.MemberName });
                    }
                    else
                    {
                        if (datas.EstablishNoSeriesCodeIDs.Count() > 0)
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return new ValidationResult("Establish No Series Code is Required", new[] { validationContext.MemberName });
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
                return new ValidationResult("Establish No Series Code is Required", new[] { validationContext.MemberName });
            }
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NoOfDigitValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DocumentProfileNoSeriesModel)validationContext.ObjectInstance;
                if (datas.NoOfDigit > 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("No Of Digit must greater than zero", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IncrementalNoValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DocumentProfileNoSeriesModel)validationContext.ObjectInstance;
                if (datas.IncrementalNo > 0)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Incremental No must greater than zero", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StartingNosValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var datas = (DocumentProfileNoSeriesModel)validationContext.ObjectInstance;
                var startNo = datas.StartingNo;
                if (!string.IsNullOrEmpty(datas.StartingNo))
                {
                    var num = new string(datas.StartingNo.Where(c => char.IsDigit(c)).ToArray());
                    if (!string.IsNullOrEmpty(num))
                    {
                        if (num == startNo)
                        {
                            datas.StartingNo = num;
                            if (datas.NoOfDigit > 0)
                            {
                                var count = datas.StartingNo?.ToString().Length;
                                if (count == datas.NoOfDigit)
                                {
                                    return ValidationResult.Success;
                                }
                                else
                                {
                                    return new ValidationResult("Must be equal to No of Digit", new[] { validationContext.MemberName });
                                }
                            }
                            else
                            {
                                return new ValidationResult("No of Digit is empty", new[] { validationContext.MemberName });
                            }
                        }
                        else
                        {
                            return new ValidationResult("Invalid Starting No", new[] { validationContext.MemberName });
                        }
                    }
                    else
                    {
                        return new ValidationResult("Invalid Starting No", new[] { validationContext.MemberName });
                    }
                }
                else
                {
                    return new ValidationResult("Invalid Starting No", new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
