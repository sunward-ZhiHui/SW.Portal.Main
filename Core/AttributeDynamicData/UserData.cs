
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.AttributeDynamicData
{
    public class UserData
    {

        [Required(ErrorMessage = "The Username value should be specified.")]
        [FieldType(FieldType.Text)]
        [Display(Name = "UserName",Description = "User Name")]
        public string Username { get; set; }

        [Required(ErrorMessage = "The Password value should be specified.")]
        [MinPasswordLength(6, "The Password must be at least 6 characters long.")]
        [FieldType(FieldType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Required(ErrorMessage = "The Email value should be specified.")]
        [Email(ErrorMessage = "The Email value is invalid.")]
        [FieldType(FieldType.Text)]
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Phone value should be specified.")]
        [FieldType(FieldType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [FieldType(FieldType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; } = new DateTime(1970, 1, 1);

        [FieldType(FieldType.CheckBox)]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = false;

        [FieldType(FieldType.RadioGroup)]
        [Display(Name = "Language")]
        public string Language { get; set; }

        [FieldType(FieldType.ComboBox)]
        [Display(Name = "Occupation")]
        public string Occupation { get; set; }

        [FieldType(FieldType.MultilineText)]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        [FieldType(FieldType.List)]
        [Display(Name = "List")]
        public string List { get; set; }
        [FieldType(FieldType.TagBox)]
        [Display(Name = "TagBox")]
        public IEnumerable<string> TagBoxList { get; set; }

    }

    public class AdditionalData
    {
        public static IEnumerable<string> Occupations { get; set; } = new List<string>() {
        "Academic",
        "Administrative",
        "Art/Entertainment",
        "College Student",
        "Community & Social",
        "Computers",
        "Education",
        "Engineering",
        "Financial Services",
        "Government",
        "High School Student",
        "Law",
        "Managerial",
        "Manufacturing",
        "Medical/Health",
        "Military",
        "Non-government Organization",
        "Other Services",
        "Professional",
        "Retail",
        "Science & Research",
        "Sports",
        "Technical",
        "University Student",
        "Web Building",
    };
    }

    public class UserEntry
    {
        public static IEnumerable<UserData> UserDatas { get; set; } = new List<UserData>() { new UserData { Username = "Sakthi", BirthDate = new DateTime(1986, 05, 20), Email = "sakthiaseer@gmail.com", IsActive = true, Language = "English" }, new UserData { Username = "Ramya", BirthDate = new DateTime(1991, 07, 08), Email = "sakthiaseer@gmail.com", IsActive = true, Language = "English" } };
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class MinPasswordLengthAttribute : ValidationAttribute
    {
        int MinLength { get; }
        public MinPasswordLengthAttribute(int minLength, string errorMsg) : base(errorMsg)
        {
            MinLength = minLength;
        }

        public override bool IsValid(object value)
        {
            return ((string)value).Length >= MinLength;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EmailAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Regex.IsMatch((string)value, @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                             + "@"
                                              + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$");
        }
    }


}

