using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailSearch
    {
        public string? BySubject { get; set; }
        public string? ByTag { get; set; }
        public IEnumerable<long> FromIds { get; set; }
        public string ByFrom { get; set; }
        public DateTime? FilterFrom { get; set; }
        public DateTime? FilterTo { get; set; }
        public string? MSearchText { get; set; }
        public long UserID { get; set; }
        public string? GroupTag { get; set; }
        public string? CategoryTag { get; set; }
        public string? ActionTag { get; set; }
        public string? Name { get; set; }
        public string? UserTag { get; set; }
        public bool UnArchive { get; set; }
        public bool IsClose { get; set; }
        public string? IsStatus { get; set; } = "All";
        public bool IsOpenandClose { get; set; } = true;
        [NotMapped]
        public IEnumerable<long?> ActionTagIds { get; set; } = new List<long?>();
        [NotMapped]
        public string? AllParticipantCondition { get; set; }
        public string? SubjectCondition { get; set; }
        public string? SubjectLike { get; set; }
        public string? GroupLike { get; set; }
        public string? CategoryLike { get; set; }
        public string? ActionLike { get; set; }
        public string? OtherLike { get; set; }
        public string? GroupCondition { get; set; }
        public string? CategoryCondition { get; set; }
        public string? ActionCondition { get; set; }
        public string? OtherTagCondition { get; set; }
        public List<SubjectFilterModel> SubjectFilters { get; set; } = new();
        public string SubjectFilterSQL { get; set; }

    }
    public class SubjectFilterModel
    {
        public string BySubject { get; set; }
        public string SubjectLike { get; set; } = "=";
        public string SubjectCondition { get; set; } = "AND";
    }

    public class DropDownModel
    {
        public string? Value { get; set; }
        public string? Text { get; set; }
    }
    public class DropDownModelSubject
    {
        public string? Value { get; set; }
        public string? Text { get; set; }
    }
}
