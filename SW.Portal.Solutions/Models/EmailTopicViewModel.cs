namespace SW.Portal.Solutions.Models
{
    public class EmailTopicViewModel
    {
        public long id { get; set; }
        public long replyId { get; set; }
        public string? topicName { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public bool? urgent { get; set; }
        public bool? isAllowParticipants { get; set; }
        public DateTime? dueDate { get; set; }
        public long? onBehalf { get; set; }
        public string? onBehalfName { get; set; }
        public DateTime? addedDate { get; set; }
        public long? addedByUserID { get; set; }
        public string? mode { get;set; }
        public long userId { get; set; }
        public int? addedDateYear { get; set; }
        public int? addedDateMonth { get; set; }
        public string? addedDateDay { get; set; }
        public string? addedTime { get; set; }
    }
}
