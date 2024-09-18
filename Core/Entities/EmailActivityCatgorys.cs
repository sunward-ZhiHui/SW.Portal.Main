using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailActivityCatgorys
    {
        [Key]
        public long ID { get; set; }
        public long TopicId { get; set; }
        //[EitherRequired("Name", "GroupTag", ErrorMessage = "Either Others or GroupTag is required.")]
        public string Name { get; set; }
        public string UserTag { get; set; }       
        public long? GroupTag { get; set; }
        public long? CategoryTag { get; set; }
        public long? ActionTag { get; set; }
        public string? GroupName { get; set; }
        public string? CategoryName { get; set; }
        public string? ActionName { get; set; }
        public string Description { get; set; }
        public DateTime AddedDate { get; set; }
        public long? ModifiedByUserID { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public Guid? SessionId { get; set; }
        public int? StatusCodeID { get; set; }
        public bool? IsTagDelete { get; set; }
        public long? AddedByUserID { get; set; }
        [NotMapped]
        public string ModifiedBy { get; set; }
        [NotMapped]
        public string AddedBy { get; set; }
        public string? ActivityType { get; set; }
        [NotMapped]
        public long? UserTagId { get; set; }
        [NotMapped]
        public long? UserTagTopicId { get; set; }
        [NotMapped]
        public long? UserTagAddedByUserID { get; set; }
        [NotMapped]
       // [Required]
        public IEnumerable<long> ActionTagIds { get; set; }
    }


    public class EitherRequiredAttribute : ValidationAttribute
    {
        private readonly string[] _propertyNames;

        public EitherRequiredAttribute(params string[] propertyNames)
        {
            _propertyNames = propertyNames;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var hasValue = false;

            foreach (var propertyName in _propertyNames)
            {
                var property = validationContext.ObjectType.GetProperty(propertyName);
                if (property != null)
                {
                    var propertyValue = property.GetValue(validationContext.ObjectInstance);
                    if (propertyValue != null && !string.IsNullOrWhiteSpace(propertyValue.ToString()))
                    {
                        hasValue = true;
                        break;
                    }
                }
            }

            if (!hasValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

}
