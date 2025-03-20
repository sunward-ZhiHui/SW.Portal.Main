using AutoMapper;
using Core.Entities;
using Core.EntityModels;
using Core.Repositories.Query;
using MailKit;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SW.Portal.Solutions.Models;
using SW.Portal.Solutions.Utils;

namespace SW.Portal.Solutions.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutlookEmailController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IGenerateDocumentNoSeriesSeviceQueryRepository _generateDocumentNoSeries;
        public OutlookEmailController(IMapper mapper, IWebHostEnvironment host, IGenerateDocumentNoSeriesSeviceQueryRepository generateDocumentNoSeries)
        {            
            _mapper = mapper;
            _hostingEnvironment = host;
            _generateDocumentNoSeries = generateDocumentNoSeries;
        }
        [HttpGet]
        [Route("ReceiveEmails")]
        public List<EmailModel> ReceiveEmails()
        {
            var emails = new List<EmailModel>();
            using (var client = new ImapClient())
            {
                client.Connect("mail.sunwardpharma.com", 143, false);

                client.Authenticate("salesorder@sunwardpharma.com", "SA@636order");

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    var attachments = new List<AttachmentModel>();
                    message.Attachments.ToList().ForEach(f =>
                    {
                        attachments.Add(new AttachmentModel
                        {

                        });

                    });
                    emails.Add(new EmailModel
                    {
                        Subject = message.Subject,
                        BodyText = message.TextBody,
                        BodyHtml = message.HtmlBody,
                        From = message.From.Mailboxes.Select(s => s.Address).Aggregate((i, j) => i + ";" + j),
                        To = message.To.Mailboxes.Select(s => s.Address).Aggregate((i, j) => i + ";" + j),
                        FromName = message.From.Mailboxes.Select(s => s.Name).Aggregate((i, j) => i + "," + j),
                        ToName = message.To.Mailboxes.Select(s => s.Name).Aggregate((i, j) => i + "," + j),
                        IsAttachment = message.Attachments.Count() > 0 ? true : false,
                        Date = message.Date,
                        MessageId = message.MessageId,
                    });
                }

                client.Disconnect(true);
            }
            return emails.OrderByDescending(d => d.Date).ToList();
        }
        [HttpGet]
        [Route("GetEmailTypeList")]
        public EmailTypeModel GetEmailTypeList()
        {
            var emailTypeModel = new EmailTypeModel();
            List<EmailModel> SalesOrderToProcess = new List<EmailModel>();
            List<EmailModel> salesOrderComplete = new List<EmailModel>();
            List<EmailModel> notRelated = new List<EmailModel>();
            //var receiveEmail = _context.ReceiveEmail.ToList();            
            using (var client = new ImapClient())
            {
                client.Connect("mail.sunwardpharma.com", 143, false);

                client.Authenticate("salesorder@sunwardpharma.com", "SA@636order");

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = inbox.GetMessage(i);
                    //var mailData = receiveEmail.Where(s => s.MessageId == message.MessageId).FirstOrDefault();
                    //if (mailData == null || mailData.EmailSalesStatusId == 1721)
                    //{
                    //    SalesOrderToProcess.Add(EmailMessage(message, mailData, 1721));
                    //}
                    //else
                    //{
                    //    if (mailData.EmailSalesStatusId == 1722)
                    //    {
                    //        salesOrderComplete.Add(EmailMessage(message, mailData, 1722));
                    //    }
                    //    if (mailData.EmailSalesStatusId == 1723)
                    //    {
                    //        notRelated.Add(EmailMessage(message, mailData, 1723));
                    //    }
                    //}
                    //if (mailData != null && mailData.SameAsSalesOrderCompleteProcess == true)
                    //{
                    //    salesOrderComplete.Add(EmailMessage(message, mailData, 1722));
                    //}
                }
                client.Disconnect(true);
            }
            emailTypeModel.SalesOrderToProcess = SalesOrderToProcess;
            emailTypeModel.SalesOrderComplete = salesOrderComplete;
            emailTypeModel.NotRelated = notRelated;
            return emailTypeModel;
        }
        private EmailModel EmailMessage(MimeMessage message, ReceiveEmail mailData, int? EmailSalesStatusId)
        {
            EmailModel emailModel = new EmailModel();
            List<EmailAttachmentModel> attachments = new List<EmailAttachmentModel>();
            foreach (MimeEntity attachment in message.Attachments)
            {
                var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;
                EmailAttachmentModel EmailAttachmentModel = new EmailAttachmentModel();
                EmailAttachmentModel.ContentType = attachment.ContentType.MimeType;
                EmailAttachmentModel.FileName = attachment.ContentDisposition.FileName;
                EmailAttachmentModel.Size = attachment.ContentDisposition.Size;
                using (var memory = new MemoryStream())
                {
                    if (attachment is MimePart)
                        ((MimePart)attachment).Content.DecodeTo(memory);
                    else
                        ((MessagePart)attachment).Message.WriteTo(memory);

                    var bytes = memory.ToArray();
                    EmailAttachmentModel.FileData = bytes;
                }
                attachments.Add(EmailAttachmentModel);
            }
            emailModel.Attachments = attachments;
            emailModel.Subject = message.Subject;
            emailModel.BodyText = message.TextBody;
            emailModel.From = message.From.Count > 0 ? message.From.Mailboxes.Select(s => s.Address).Aggregate((i, j) => i + ";" + j) : "";
            emailModel.To = message.To.Count > 0 ? message.To.Mailboxes.Select(s => s.Address).Aggregate((i, j) => i + ";" + j) : "";
            emailModel.FromName = message.From.Count > 0 ? message.From.Mailboxes.Select(s => s.Name).Aggregate((i, j) => i + "," + j) : "";
            emailModel.ToName = message.To.Count > 0 ? message.To.Mailboxes.Select(s => s.Name).Aggregate((i, j) => i + "," + j) : "";
            emailModel.CC = message.Cc.Count > 0 ? message.Cc.Mailboxes.Select(s => s.Address).Aggregate((i, j) => i + ";" + j) : "";
            emailModel.CCName = message.Cc.Count > 0 ? message.Cc.Mailboxes.Select(s => s.Name).Aggregate((i, j) => i + "," + j) : "";
            emailModel.IsAttachment = message.Attachments.Count() > 0 ? true : false;
            emailModel.Date = message.Date;
            emailModel.MessageId = message.MessageId;
            emailModel.EmailSalesStatusId = mailData != null ? mailData.EmailSalesStatusId : EmailSalesStatusId;
            emailModel.ReceiveEmailId = mailData != null ? mailData.ReceiveEmailId : 0;
            emailModel.IsAcknowledgement = mailData != null ? mailData.IsAcknowledgement : false;
            emailModel.Sonumber = mailData != null ? mailData.Sonumber : "";
            emailModel.NotRelatedDescription = mailData != null ? mailData.NotRelatedDescription : "";
            emailModel.SameAsSalesOrderCompleteProcess = mailData != null ? mailData.SameAsSalesOrderCompleteProcess : false;
            emailModel.SameAsSalesOrderCompleteProcessFlag = mailData != null ? (mailData.SameAsSalesOrderCompleteProcess == true ? "Yes" : "No") : "No";
            emailModel.SalesOrderId = mailData != null ? mailData.SalesOrderId : null;
            emailModel.DocumentId = mailData != null ? mailData.DocumentId : null;
            //emailModel.SessionId = mailData != null ? mailData.SessionId : null;
            emailModel.Description = mailData != null ? mailData.Description : "";
            emailModel.HtmlFileName = mailData != null ? mailData.HtmlFileName : "";
            emailModel.FileProfileTypeId = mailData != null ? mailData.FileProfileTypeId : null;
            var visitor = new HtmlPreviewVisitor();
            message.Accept(visitor);
            emailModel.BodyHtml = visitor.HtmlBody;
            return emailModel;
        }
        [HttpPost]
        [Route("InsertReceiveEmail")]
        public EmailModel Post(EmailModel value)
        {
            var sessionId = Guid.NewGuid();
            long? mainDocumentId = null;
            if (value.Type == "Document")
            {
                var serverPath = _hostingEnvironment.ContentRootPath + @"\AppUpload\" + sessionId + ".html";
                System.IO.File.WriteAllText(serverPath, value.BodyHtml);
                FileStream stream = System.IO.File.OpenRead(serverPath);
                var br = new BinaryReader(stream);
                Byte[] documents = br.ReadBytes((Int32)stream.Length);
                var compressedData = DocumentZipUnZip.Zip(documents);//Compress(document);
                //var profile = _context.FileProfileType.FirstOrDefault(f => f.FileProfileTypeId == value.FileProfileTypeId);
                string profileNo = "";
                //if (profile != null)
                //{
                //    if (value.AddedByUserID != null)
                //    {
                //        //profileNo = _generateDocumentNoSeries.GenerateDocumentNo(new DocumentNoSeriesModel { ProfileID = profile.ProfileId, Title = profile.Name, AddedByUserID = value.AddedByUserID, StatusCodeID = 710 });

                //    }
                //}
                var document = new Documents
                {
                    FileName = value.HtmlFileName + ".html",
                    ContentType = "text/html",
                    FileData = compressedData,
                    FileSize = new System.IO.FileInfo(serverPath).Length,
                    UploadDate = DateTime.Now,
                    AddedDate = DateTime.Now,
                    //AddedByUserId = value.AddedByUserID,
                    SessionId = sessionId,
                    IsTemp = true,
                    IsCompressed = true,
                    IsLatest = true,
                    ProfileNo = profileNo,
                    FilterProfileTypeId = value.FileProfileTypeId,
                };
                stream.Close();
                //_context.Documents.Add(document);
                //_context.SaveChanges();
                mainDocumentId = document.DocumentId;
                System.IO.File.Delete(serverPath);
                if (value.Attachments.Count > 0)
                {
                    string profileNos = "";
                    value.Attachments.ForEach(a =>
                    {
                        //if (profile != null)
                        //{
                        //    if (value.AddedByUserID != null)
                        //    {
                        //        //profileNos = _generateDocumentNoSeries.GenerateDocumentNo(new DocumentNoSeriesModel { ProfileID = profile.ProfileId, Title = profile.Name, AddedByUserID = value.AddedByUserID, StatusCodeID = 710 });

                        //    }
                        //}
                        var compressedData = DocumentZipUnZip.Zip(a.FileData);//Compress(document);
                        var documents = new Documents
                        {
                            FileName = a.FileName,
                            ContentType = a.ContentType,
                            FileData = compressedData,
                            FileSize = a.Size,
                            UploadDate = DateTime.Now,
                            AddedDate = DateTime.Now,
                            SessionId = sessionId,
                            FilterProfileTypeId = value.FileProfileTypeId,
                            ProfileNo = profileNos,
                            IsLatest = true,
                            //AddedByUserId = value.AddedByUserID,
                            IsCompressed = true,
                            FileIndex = 0,
                            IsMainTask = false,
                        };
                        //_context.Documents.Add(documents);
                        //_context.SaveChanges();
                        var linkDocumentId = documents.DocumentId;
                        var DocumentLink = new DocumentLink
                        {
                            DocumentId = mainDocumentId,
                            //AddedByUserId = value.AddedByUserID,
                            AddedDate = DateTime.Now,
                            LinkDocumentId = linkDocumentId,
                            FileProfieTypeId = value.FileProfileTypeId,
                            DocumentPath = "Email Link Document",
                        };
                        //_context.DocumentLink.Add(DocumentLink);
                        //_context.SaveChanges();
                    });
                }
            }
            var receiveEmail = new ReceiveEmail
            {
                Subject = value.Subject,
                MessageId = value.MessageId,
                IsAcknowledgement = value.IsAcknowledgement,
                EmailSalesStatusId = value.EmailSalesStatusId,
                Sonumber = value.Sonumber,
                AddedDate = DateTime.Now,
                //AddedByUserId = value.AddedByUserID.Value,
                NotRelatedDescription = value.NotRelatedDescription,
                SameAsSalesOrderCompleteProcess = value.SameAsSalesOrderCompleteProcessFlag == "Yes" ? true : false,
                SalesOrderId = value.SalesOrderId,
                HtmlFileName = value.HtmlFileName,
                Description = value.Description,
                SessionId = sessionId,
                DocumentId = mainDocumentId,
                FileProfileTypeId = value.FileProfileTypeId,
            };
            //_context.ReceiveEmail.Add(receiveEmail);
            //_context.SaveChanges();
            value.ReceiveEmailId = receiveEmail.ReceiveEmailId;
            //value.SessionId = sessionId;
            value.DocumentId = receiveEmail.DocumentId;
            return value;
        }
        [HttpPut]
        [Route("UpdateReceiveEmail")]
        public EmailModel Put(EmailModel value)
        {
            //value.SessionId ??= Guid.NewGuid();
            //var receiveEmail = _context.ReceiveEmail.SingleOrDefault(p => p.ReceiveEmailId == value.ReceiveEmailId);
            //receiveEmail.Subject = value.Subject;
            //receiveEmail.MessageId = value.MessageId;
            //receiveEmail.IsAcknowledgement = value.IsAcknowledgement;
            //receiveEmail.EmailSalesStatusId = value.EmailSalesStatusId;
            //receiveEmail.Sonumber = value.Sonumber;
            //receiveEmail.ModifiedDate = DateTime.Now;
            //receiveEmail.ModifiedByUserId = value.ModifiedByUserID.Value;
            //receiveEmail.NotRelatedDescription = value.NotRelatedDescription;
            //receiveEmail.SameAsSalesOrderCompleteProcess = value.SameAsSalesOrderCompleteProcessFlag == "Yes" ? true : false;
            //receiveEmail.SalesOrderId = value.SalesOrderId;
            //receiveEmail.SessionId = value.SessionId;
            //_context.SaveChanges();
            return value;
        }
    }
}
