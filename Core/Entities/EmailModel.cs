using Core.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class EmailModel
    {
        public string MessageId { get; set; }
        public string Subject { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string FromName { get; set; }
        public string ToName { get; set; }
        public string CC { get; set; }
        public string CCName { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public bool IsAttachment { get; set; }
        public DateTimeOffset Date { get; set; }
        public long ReceiveEmailId { get; set; }
        public int? EmailSalesStatusId { get; set; }
        public bool? IsAcknowledgement { get; set; }
        public string Sonumber { get; set; }
        public string Body { get; set; }      
        public List<EmailAttachmentModel> Attachments { get; set; }
        public string NotRelatedDescription { get; set; }
        public bool? SameAsSalesOrderCompleteProcess { get; set; }
        public string SameAsSalesOrderCompleteProcessFlag { get; set; }
        public long? SalesOrderId { get; set; }
        public string HtmlFileName { get; set; }
        public string Description { get; set; }
        public long? EmailCategoryId { get; set; }
        public long? DocumentId { get; set; }
        public string Type { get; set; }
        public long? FileProfileTypeId { get; set; }
        public bool IsExpanded { get; set; } = false;
    }
    public class EmailAttachmentModel
    {
        public string SafeFileName { get; set; }
        public long? Size { get; set; }
        public string To { get; set; }
        public string BodyText { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
        public byte[] FileData { get; set; }
        public string DownloadUrl { get; set; }
    }
    public class EmailTypeModel
    {
        public List<EmailModel> SalesOrderToProcess { get; set; }
        public List<EmailModel> SalesOrderComplete { get; set; }
        public List<EmailModel> NotRelated { get; set; }
    }
}