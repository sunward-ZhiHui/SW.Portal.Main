using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.EntityModels
{
    public class SearchModel
    {
        public long Id { get; set; }
        public string Action { get; set; }
        public string MethodName { get; set; }
        public string CompanyName { get; set; }
        public string SearchString { get; set; }
        public string Search { get; set; }
        public string SortCol { get; set; }
        public string SortBy { get; set; }
        public long UserID { get; set; }

        public bool IncludeHidden { get; set; }
        public long? MasterTypeID { get; set; }
        public DateTime? Date { get; set; }
        public Guid? SessionID { get; set; }
        public string ScreenID { get; set; }
        public long? ParentID { get; set; }
        public long? FolderId { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int RowCount { get; set; }
        public List<string> FilterFields { get; set; }
        public List<long?> Ids { get; set; }

        public long? PrimaryId { get; set; }
        public string PrimaryColumn { get; set; }
        public long? ClassificationId { get; set; }
        public int? ClassificationTypeId { get; set; }
        public string ProfileReferenceNo { get; set; }
        public bool IsHeader { get; set; }
        public int? TypeID { get; set; }
        public string NavType { get; set; }
        public long? FileProfileTypeId { get; set; }
        public string ContentType { get; set; }
        public DateTime? FromMonth { get; set; }
        public DateTime? ToMonth { get; set; }
        public string FileName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string IsSwstaff { get; set; }
        public List<long?> CompanyId { get; set; }
        public List<string> LocationId { get; set; }
        public string Temperature { get; set; }
        public string Questionnaire { get; set; }
        public bool? AllLocation { get; set; }
        public bool? AllVisitorCompany { get; set; }
        public long? PlantId { get; set; }
        public int? MovementStatusId { get; set; }
        public string TesKitResult { get; set; }
        public bool? IsDuplicate { get; set; }
        public long? WikiCategoryId { get; set; }
        public List<long?> TypeOfEventId { get; set; }
        public List<long?> LocationEventId { get; set; }
        public List<long?> CalenderStatusId { get; set; }
        public bool? OverDue { get; set; }
        public string Subject { get; set; }
        public long? TypeOfServiceId { get; set; }
        public long? TypeOfEventIds { get; set; }
        public DateTime? EventDate { get; set; }
        public long? CalenderStatusIds { get; set; }
        public DateTime? DueDate { get; set; }
        public string BaseUrl { get; set; }
    }

    public enum SearchAction
    {
        Next = 1,
        Previous = 2,
        Last = 3,
        First = 4,
    }
}
