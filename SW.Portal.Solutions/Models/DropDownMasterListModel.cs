using Core.Entities;

namespace SW.Portal.Solutions.Models
{
    public class DropDownMasterListModel
    {
        public long Id { get; set; }
        public string? Value { get; set; }
        public string? Text { get; set; }
        public List<DropDownMasterListModel?> ActivityStatusItem { get; set; } = new List<DropDownMasterListModel?>();
        public List<DropDownMasterListModel?> ActivityResult { get; set; } = new List<DropDownMasterListModel?>();
        public List<DropDownMasterListModel?> ActivityMaster { get; set; } = new List<DropDownMasterListModel?>();
    }
}
