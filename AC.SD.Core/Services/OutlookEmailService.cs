using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using Core.Entities;
using System.IO;

namespace AC.SD.Core.Services
{
    public class OutlookEmailService
    {
        private readonly AttachmentService _attachmentService;
        private readonly string _imapServer = "imap.gmail.com";
        //private readonly string _imapServer = "mail.sunwardpharma.com";
        private readonly int _imapPort = 993;
        //private readonly int _imapPort = 143;
        private readonly string _emailAddress = "demoinfotech001@gmail.com";
        private readonly string _emailPassword = "ruke ncnc qfbh jahu";
        private readonly string localAttachmentPath = "C:\\EmailAttachments\\";
        private static readonly Dictionary<string, (byte[], string)> _attachmentStore = new();

        public OutlookEmailService(AttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsync2()
        {
            var emails = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");

                    // Connect securely using SSL
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected successfully!");

                    // Authenticate user
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);
                    Console.WriteLine("Authentication successful!");

                    // Open the Inbox folder
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);

                    Console.WriteLine($"Inbox contains {inbox.Count} messages.");

                    var results = await inbox.SearchAsync(SearchQuery.All);
                    //var results = await inbox.SearchAsync(SearchQuery.DeliveredAfter(DateTime.Now.AddDays(-7)));

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);

                        // Process attachments
                        var attachments = new List<EmailAttachmentModel>();
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                attachments.Add(new EmailAttachmentModel
                                {
                                    FileName = part.FileName,
                                    ContentType = part.ContentType.MimeType,
                                    //Content = part.ContentDisposition?.Disposition

                                });
                            }
                        }

                        // Add email to the list
                        emails.Add(new EmailModel
                        {
                            Subject = message.Subject,
                            BodyText = message.TextBody,
                            BodyHtml = message.HtmlBody,
                            From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                            To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                            FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                            ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                            IsAttachment = message.Attachments.Any(),
                            Date = message.Date.DateTime,
                            MessageId = message.MessageId,
                            Attachments = attachments
                        });
                    }

                    // Disconnect cleanly
                    await client.DisconnectAsync(true);
                    Console.WriteLine("Disconnected from IMAP server.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }


            var lsls = emails.ToList();

            // Sort emails by date (latest first)
            return emails.OrderByDescending(d => d.Date).ToList();
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsync1()
        {
            var emails = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");

                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected successfully!");

                    await client.AuthenticateAsync(_emailAddress, _emailPassword);
                    Console.WriteLine("Authentication successful!");

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);

                    Console.WriteLine($"Inbox contains {inbox.Count} messages.");

                    var results = await inbox.SearchAsync(SearchQuery.All);

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);

                        // Process attachments
                        var attachments = new List<EmailAttachmentModel>();
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    var attachmentBytes = memoryStream.ToArray();

                                    // Save to local storage or generate a URL
                                    string filePath = Path.Combine("wwwroot", "attachments", part.FileName);
                                    await File.WriteAllBytesAsync(filePath, attachmentBytes);

                                    // Generate a relative download URL
                                    string downloadUrl = $"/attachments/{part.FileName}";

                                    attachments.Add(new EmailAttachmentModel
                                    {
                                        FileName = part.FileName,
                                        ContentType = part.ContentType.MimeType,
                                        Content = attachmentBytes,
                                        DownloadUrl = downloadUrl
                                    });
                                }
                            }
                        }

                        emails.Add(new EmailModel
                        {
                            Subject = message.Subject,
                            BodyText = message.TextBody,
                            BodyHtml = message.HtmlBody,
                            From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                            To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                            FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                            ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                            IsAttachment = attachments.Any(),
                            Date = message.Date.DateTime,
                            MessageId = message.MessageId,
                            Attachments = attachments
                        });
                    }

                    await client.DisconnectAsync(true);
                    Console.WriteLine("Disconnected from IMAP server.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails.OrderByDescending(d => d.Date).ToList();
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsyncold()
        {
            var emails = new List<EmailModel>();


            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var results = await inbox.SearchAsync(SearchQuery.All);


                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        var attachments = new List<EmailAttachmentModel>();


                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    var attachmentBytes = memoryStream.ToArray();

                                    // Generate safe filename
                                    string safeFileName = $"{Guid.NewGuid()}_{part.FileName}";

                                    // Ensure the directory exists
                                    if (!Directory.Exists(localAttachmentPath))
                                    {
                                        Directory.CreateDirectory(localAttachmentPath);
                                    }

                                    // Save file to local disk
                                    string filePath = Path.Combine(localAttachmentPath, safeFileName);
                                    await File.WriteAllBytesAsync(filePath, attachmentBytes);
                                    Console.WriteLine($"Attachment saved: {filePath}");

                                    // Generate API download URL
                                    //string downloadUrl = $"/api/files/download?filename={safeFileName}";

                                    string token = Guid.NewGuid().ToString();
                                    _attachmentStore[token] = (attachmentBytes, part.ContentType.MimeType);
                                    string downloadUrl = $"/api/files/download/{token}/{Uri.EscapeDataString(part.FileName)}";


                                    //string base64Data = Convert.ToBase64String(attachmentBytes);
                                    //string downloadUrl = $"/api/files/download?filename={Uri.EscapeDataString(part.FileName)}&data={Uri.EscapeDataString(base64Data)}";


                                    attachments.Add(new EmailAttachmentModel
                                    {
                                        FileName = part.FileName,
                                        ContentType = part.ContentType.MimeType,
                                        DownloadUrl = downloadUrl
                                    });
                                }
                            }
                        }

                        emails.Add(new EmailModel
                        {
                            Subject = message.Subject,
                            BodyText = message.TextBody,
                            BodyHtml = message.HtmlBody,
                            From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                            To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                            FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                            ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                            IsAttachment = message.Attachments.Any(),
                            Date = message.Date.DateTime,
                            MessageId = message.MessageId,
                            Attachments = attachments
                        });
                    }

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails.OrderByDescending(d => d.Date).ToList();
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsync()
        {
            var emails = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var results = await inbox.SearchAsync(MailKit.Search.SearchQuery.All);

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        var attachments = new List<EmailAttachmentModel>();

                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    byte[] attachmentBytes = memoryStream.ToArray();

                                    // Store in memory and get a token
                                    string token = _attachmentService.StoreAttachment(attachmentBytes, part.ContentType.MimeType);

                                    // Generate a download URL using the token
                                    string downloadUrl = $"/api/files/download/{token}/{Uri.EscapeDataString(part.FileName)}";

                                    attachments.Add(new EmailAttachmentModel
                                    {
                                        FileName = part.FileName,
                                        ContentType = part.ContentType.MimeType,
                                        DownloadUrl = downloadUrl
                                    });
                                }
                            }
                        }

                        emails.Add(new EmailModel
                        {
                            Subject = message.Subject,
                            BodyText = message.TextBody,
                            BodyHtml = message.HtmlBody,
                            From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                            To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                            FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                            ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                            IsAttachment = message.Attachments.Any(),
                            Date = message.Date.DateTime,
                            MessageId = message.MessageId,
                            Attachments = attachments
                        });
                    }

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails.OrderByDescending(d => d.Date).ToList();
        }


    }

}
