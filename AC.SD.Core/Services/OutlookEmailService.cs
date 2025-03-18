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
using MailKit.Net.Smtp;
using static AC.SD.Core.Pages.Email.OutlookEmailPage;
using Microsoft.AspNetCore.Http;

namespace AC.SD.Core.Services
{
    public class OutlookEmailService
    {
        private readonly string _smtpServer = "smtp.gmail.com";
        private readonly AttachmentService _attachmentService;
        private readonly string _imapServer = "imap.gmail.com";
        //private readonly string _imapServer = "mail.sunwardpharma.com";
        private readonly int _imapPort = 993;
        private readonly int _smtpPort = 587;
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

        public async Task<List<EmailModel>> ReceiveEmailsAsyncvijay()
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

        public async Task<List<EmailModel>> ReceiveEmailsAsyncno()
        {
            var emails = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    // Connect to IMAP Server
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    // Open Inbox
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var results = await inbox.SearchAsync(MailKit.Search.SearchQuery.All);

                    var emailDict = new Dictionary<string, EmailModel>();

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        var messageId = message.MessageId;
                        var inReplyTo = message.InReplyTo;
                        var attachments = new List<EmailAttachmentModel>();

                        // Extract attachments
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    byte[] attachmentBytes = memoryStream.ToArray();

                                    // Store in memory and generate a token
                                    string token = _attachmentService.StoreAttachment(attachmentBytes, part.ContentType.MimeType);

                                    // Generate a download URL
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

                        // Create Email Model
                        var email = new EmailModel
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
                            MessageId = messageId,
                            Attachments = attachments,
                            Replies = new List<EmailReply>() // Initialize replies list
                        };

                        // Handle Replies
                        if (!string.IsNullOrEmpty(inReplyTo) && emailDict.ContainsKey(inReplyTo))
                        {
                            emailDict[inReplyTo].Replies.Add(new EmailReply
                            {
                                Body = message.TextBody,
                                Date = message.Date.DateTime,
                                From = email.FromName
                            });
                        }
                        else
                        {
                            emailDict[messageId] = email;
                        }
                    }

                    // Convert dictionary to sorted list
                    emails = emailDict.Values.OrderByDescending(d => d.Date).ToList();

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails;
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsync1234()
        {
            var emails = new List<EmailModel>();
            var emailDict = new Dictionary<string, EmailModel>();

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
                        var messageId = message.MessageId;
                        var inReplyTo = message.InReplyTo;
                        var references = message.References?.ToArray();

                        // Fallback: If InReplyTo is null, use the last message in References
                        if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
                        {
                            inReplyTo = references.Last(); // Use the last message in References
                        }

                        var attachments = new List<EmailAttachmentModel>();

                        // Extract attachments
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    byte[] attachmentBytes = memoryStream.ToArray();

                                    string token = _attachmentService.StoreAttachment(attachmentBytes, part.ContentType.MimeType);
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

                        // Create Email Model
                        var email = new EmailModel
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
                            MessageId = messageId,
                            InReplyTo = inReplyTo, // FIX: Ensure InReplyTo is set
                            Attachments = attachments,
                            Replies = new List<EmailReply>()
                        };

                        emailDict[messageId] = email; // Store the email
                    }

                    // Second pass: Attach replies to their parent emails
                    foreach (var email in emailDict.Values)
                    {
                        if (!string.IsNullOrEmpty(email.InReplyTo) && emailDict.TryGetValue(email.InReplyTo, out var parentEmail))
                        {
                            parentEmail.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                Date = email.Date.DateTime,
                                From = email.FromName
                            });

                            Console.WriteLine($"Reply added: {email.Subject} -> Parent: {parentEmail.Subject}");
                        }
                        else if (!string.IsNullOrEmpty(email.InReplyTo))
                        {
                            Console.WriteLine($"Warning: Parent email with MessageId '{email.InReplyTo}' not found.");
                        }
                    }

                    // Convert dictionary to sorted list
                    emails = emailDict.Values.OrderByDescending(d => d.Date).ToList();

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails;
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsyncv()
        {
            var emails = new List<EmailModel>();
            var emailDict = new Dictionary<string, EmailModel>();
            var orphanReplies = new List<EmailModel>(); // Store orphan replies

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
                        var messageId = message.MessageId;
                        var inReplyTo = message.InReplyTo;
                        var references = message.References?.ToArray();

                        // Fallback: If InReplyTo is null, use the last message in References
                        if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
                        {
                            inReplyTo = references.Last();
                        }

                        // Debugging logs
                        Console.WriteLine($"Processing: {message.Subject}");
                        Console.WriteLine($"MessageId: {messageId}");
                        Console.WriteLine($"InReplyTo: {inReplyTo}");
                        if (references != null && references.Length > 0)
                        {
                            Console.WriteLine($"References: {string.Join(", ", references)}");
                        }

                        var attachments = new List<EmailAttachmentModel>();

                        // Extract attachments
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    byte[] attachmentBytes = memoryStream.ToArray();

                                    string token = _attachmentService.StoreAttachment(attachmentBytes, part.ContentType.MimeType);
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

                        // Create Email Model
                        var email = new EmailModel
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
                            MessageId = messageId,
                            InReplyTo = inReplyTo,
                            Attachments = attachments,
                            Replies = new List<EmailReply>()
                        };

                        emailDict[messageId] = email; // Store the email
                    }

                    // Sort emails by date before processing replies
                    var sortedEmails = emailDict.Values.OrderBy(e => e.Date).ToList();

                    foreach (var email in sortedEmails)
                    {
                        if (!string.IsNullOrEmpty(email.InReplyTo) && emailDict.TryGetValue(email.InReplyTo, out var parentEmail))
                        {
                            parentEmail.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                Date = email.Date.DateTime,
                                From = email.FromName
                            });

                            Console.WriteLine($"✅ Reply linked: {email.Subject} -> Parent: {parentEmail.Subject}");
                        }
                        else if (!string.IsNullOrEmpty(email.InReplyTo))
                        {
                            orphanReplies.Add(email); // Store orphan replies
                            Console.WriteLine($"⚠️ Orphan reply detected: {email.Subject} (InReplyTo: {email.InReplyTo})");
                        }
                    }

                    // Second pass: Try linking orphans again
                    foreach (var email in orphanReplies)
                    {
                        if (emailDict.TryGetValue(email.InReplyTo, out var parentEmail))
                        {
                            parentEmail.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                Date = email.Date.DateTime,
                                From = email.FromName
                            });

                            Console.WriteLine($"🔄 Orphan Reply linked: {email.Subject} -> Parent: {parentEmail.Subject}");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Still missing parent: {email.InReplyTo}");
                        }
                    }

                    // Convert dictionary to sorted list
                    emails = emailDict.Values.OrderByDescending(d => d.Date).ToList();

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails;
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsync()
        {
            var emails = new List<EmailModel>();
            var emailDict = new Dictionary<string, EmailModel>();
            var orphanReplies = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    // Connect and authenticate
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var results = await inbox.SearchAsync(SearchQuery.All);

                    var tempEmailList = new List<MimeMessage>();

                    // Fetch and store all emails first
                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        tempEmailList.Add(message);
                    }

                    // Sort emails by date before processing
                    tempEmailList = tempEmailList.OrderBy(m => m.Date).ToList();

                    // First pass: Store emails by MessageId
                    foreach (var message in tempEmailList)
                    {
                        var messageId = message.MessageId;
                        var inReplyTo = message.InReplyTo;
                        var references = message.References?.ToArray();

                        // Use last reference if InReplyTo is missing
                        if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
                        {
                            inReplyTo = references.Last();
                        }

                        var attachments = new List<EmailAttachmentModel>();

                        // Extract attachments
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    byte[] attachmentBytes = memoryStream.ToArray();

                                    string token = _attachmentService.StoreAttachment(attachmentBytes, part.ContentType.MimeType);
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

                        // Create Email Model
                        var email = new EmailModel
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
                            MessageId = messageId,
                            InReplyTo = inReplyTo,
                            Attachments = attachments,
                            Replies = new List<EmailReply>()
                        };

                        emailDict[messageId] = email;
                    }

                    // Second pass: Link replies to parent emails
                    foreach (var email in emailDict.Values.OrderBy(e => e.Date))
                    {
                        if (!string.IsNullOrEmpty(email.InReplyTo))
                        {
                            // Try linking using InReplyTo
                            if (emailDict.TryGetValue(email.InReplyTo, out var parentEmail))
                            {
                                parentEmail.Replies.Add(new EmailReply
                                {
                                    Body = email.BodyText,
                                    Date = email.Date.DateTime,
                                    From = email.FromName
                                });

                                Console.WriteLine($"✅ Reply linked: {email.Subject} -> Parent: {parentEmail.Subject}");
                            }
                            else
                            {
                                // Store as orphan reply for later processing
                                orphanReplies.Add(email);
                                Console.WriteLine($"⚠️ Orphan reply detected: {email.Subject} (InReplyTo: {email.InReplyTo})");
                            }
                        }
                    }

                    // Third pass: Try linking orphan replies again
                    foreach (var email in orphanReplies)
                    {
                        var possibleParent = emailDict.Values.FirstOrDefault(e => email.InReplyTo.Contains(e.MessageId));
                        if (possibleParent != null)
                        {
                            possibleParent.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                Date = email.Date.DateTime,
                                From = email.FromName
                            });

                            Console.WriteLine($"Orphan Reply linked: {email.Subject} -> Parent: {possibleParent.Subject}");
                        }
                        else
                        {
                            Console.WriteLine($"Still missing parent: {email.InReplyTo}");
                        }
                    }

                    // Convert dictionary to sorted list
                    emails = emailDict.Values.OrderByDescending(d => d.Date).ToList();

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails;
        }
        public async Task<List<EmailModel>> ReceiveEmailsAsyncrr()
        {
            var emails = new List<EmailModel>();
            var emailDict = new Dictionary<string, EmailModel>();
            var orphanReplies = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var results = await inbox.SearchAsync(SearchQuery.All);

                    var tempEmailList = new List<MimeMessage>();

                    // Fetch all emails
                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        tempEmailList.Add(message);
                    }

                    tempEmailList = tempEmailList.OrderBy(m => m.Date).ToList();

                    // First pass: Store emails by MessageId
                    foreach (var message in tempEmailList)
                    {
                        var messageId = message.MessageId;
                        var inReplyTo = message.InReplyTo;
                        var references = message.References?.ToArray();

                        if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
                        {
                            inReplyTo = references.Last();
                        }

                        // Extract attachments
                        var attachments = new List<EmailAttachmentModel>();
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart part)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await part.Content.DecodeToAsync(memoryStream);
                                    byte[] attachmentBytes = memoryStream.ToArray();

                                    string token = _attachmentService.StoreAttachment(attachmentBytes, part.ContentType.MimeType);
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

                        var email = new EmailModel
                        {
                            Subject = message.Subject,
                            BodyText = message.TextBody,
                            BodyHtml = message.HtmlBody,
                            From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                            To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                            FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                            ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                            Date = message.Date.DateTime,
                            MessageId = messageId,
                            InReplyTo = inReplyTo,
                            Attachments = attachments,  // ✅ Store extracted attachments
                            IsAttachment = attachments.Any(),
                            Replies = new List<EmailReply>()
                        };

                        emailDict[messageId] = email;
                    }

                    // Second pass: Link replies to parent emails
                    foreach (var email in emailDict.Values.OrderBy(e => e.Date))
                    {
                        if (!string.IsNullOrEmpty(email.InReplyTo))
                        {
                            if (emailDict.TryGetValue(email.InReplyTo, out var parentEmail))
                            {
                                parentEmail.Replies.Add(new EmailReply
                                {
                                    Body = email.BodyText,
                                    BodyHtml = email.BodyHtml,
                                    Date = email.Date.DateTime,
                                    From = email.FromName
                                });

                                // ✅ Inherit attachments from replies if parent lacks them
                                if (!parentEmail.IsAttachment && email.IsAttachment)
                                {
                                    parentEmail.Attachments.AddRange(email.Attachments);
                                    parentEmail.IsAttachment = true;
                                }
                            }
                            else
                            {
                                orphanReplies.Add(email);
                            }
                        }
                    }

                    // Third pass: Link orphan replies if possible
                    foreach (var email in orphanReplies)
                    {
                        var possibleParent = emailDict.Values.FirstOrDefault(e => email.InReplyTo.Contains(e.MessageId));
                        if (possibleParent != null)
                        {
                            possibleParent.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                BodyHtml = email.BodyHtml,
                                Date = email.Date.DateTime,
                                From = email.FromName
                            });

                            // ✅ Inherit attachments from orphan replies
                            if (!possibleParent.IsAttachment && email.IsAttachment)
                            {
                                possibleParent.Attachments.AddRange(email.Attachments);
                                possibleParent.IsAttachment = true;
                            }
                        }
                    }

                    // Group emails by their normalized subject
                    var groupedEmails = new Dictionary<string, EmailModel>();

                    foreach (var email in emailDict.Values.OrderBy(e => e.Date))
                    {
                        var normalizedSubject = NormalizeSubject(email.Subject);

                        if (!groupedEmails.TryGetValue(normalizedSubject, out var mainEmail))
                        {
                            groupedEmails[normalizedSubject] = email;
                        }
                        else
                        {
                            mainEmail.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                BodyHtml = email.BodyHtml,
                                Date = email.Date.DateTime,
                                From = email.FromName
                            });

                            // ✅ Attachments should be stored in the main email if replies have them
                            if (!mainEmail.IsAttachment && email.IsAttachment)
                            {
                                mainEmail.Attachments.AddRange(email.Attachments);
                                mainEmail.IsAttachment = true;
                            }
                        }
                    }

                    emails = groupedEmails.Values.OrderByDescending(d => d.Date).ToList();

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails;
        }

        private string NormalizeSubject(string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return subject;

            string normalized = subject.Trim();
            while (normalized.StartsWith("Re: ", StringComparison.OrdinalIgnoreCase) ||
                   normalized.StartsWith("Fwd: ", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring(4).Trim();
            }
            return normalized;
        }
        public async Task<List<EmailReply>> LoadEmailRepliesAsync(string messageId)
        {
            var replies = new List<EmailReply>();

            using (var client = new ImapClient())
            {
                try
                {
                    // 🔹 Connect to IMAP
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);

                    // 🔹 Search for emails where "References" or "In-Reply-To" contains messageId
                    var results = await inbox.SearchAsync(SearchQuery.HeaderContains("References", messageId)
                                              .Or(SearchQuery.HeaderContains("In-Reply-To", messageId)));

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);

                        // 🔹 Create EmailReply object
                        var reply = new EmailReply
                        {
                            Body = message.TextBody,
                            Date = message.Date.DateTime,
                            From = string.Join(",", message.From.Mailboxes.Select(m => m.Name))
                        };

                        replies.Add(reply);
                    }

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading replies: {ex.Message}");
                }
            }

            return replies;
        }

        public async Task<bool> SendEmailAsync(EmailViewModel email)
        {
            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailAddress, _emailAddress));
            message.To.Add(new MailboxAddress(email.To, email.To));
            message.Subject = "Re: " + email.Subject;
            message.Body = new TextPart("html") { Text = email.ReplyBody };
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        public async Task<bool> DeleteEmailAsync1(string messageId)
        {
            using var client = new ImapClient();
            await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            var inbox = client.Inbox;
            await inbox.OpenAsync(FolderAccess.ReadWrite);
            var uid = inbox.Search(MailKit.Search.SearchQuery.HeaderContains("Message-ID", messageId)).FirstOrDefault();
            if (uid != null)
            {
                await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, true);
                await inbox.ExpungeAsync();
            }
            await client.DisconnectAsync(true);
            return true;
        }
        public async Task DeleteEmailAsync(string messageId)
        {
            using (var client = new ImapClient())
            {
                try
                {
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    await client.AuthenticateAsync(_emailAddress, _emailPassword);

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadWrite);

                    var uids = await inbox.SearchAsync(MailKit.Search.SearchQuery.HeaderContains("Message-ID", messageId));
                    foreach (var uid in uids)
                    {
                        await inbox.AddFlagsAsync(uid, MessageFlags.Deleted, true);
                    }

                    await inbox.ExpungeAsync();
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting email: {ex.Message}");
                }
            }
        }

        public async Task<bool> SendEmailAsyncold(string to, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient();
                smtpClient.Connect(_smtpServer, 587, SecureSocketOptions.StartTls);
                smtpClient.Authenticate(_emailAddress, _emailPassword);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Me", _emailAddress));
                message.To.Add(new MailboxAddress(to, to));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                await smtpClient.SendAsync(message);
                await smtpClient.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        //public async Task SendReplyAsync(string to, string subject, string body)
        //{
        //    using (var client = new SmtpClient())
        //    {
        //        try
        //        {
        //            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
        //            await client.AuthenticateAsync(_emailAddress, _emailPassword);

        //            var message = new MimeMessage();
        //            message.From.Add(new MailboxAddress(_emailAddress));
        //            message.To.Add(new MailboxAddress(to));
        //            message.Subject = "Re: " + subject;
        //            message.Body = new TextPart("html") { Text = body };

        //            await client.SendAsync(message);
        //            await client.DisconnectAsync(true);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error sending reply: {ex.Message}");
        //        }
        //    }
        //}

        //public async Task ForwardEmailAsync(string to, string subject, string body, List<EmailAttachmentModel> attachments)
        //{
        //    using (var client = new SmtpClient())
        //    {
        //        try
        //        {
        //            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
        //            await client.AuthenticateAsync(_emailAddress, _emailPassword);

        //            var message = new MimeMessage();
        //            message.From.Add(new MailboxAddress(_emailAddress));
        //            message.To.Add(new MailboxAddress(to));
        //            message.Subject = "Fwd: " + subject;

        //            var bodyBuilder = new BodyBuilder { HtmlBody = body };

        //            // Attach files
        //            if (attachments != null)
        //            {
        //                foreach (var attachment in attachments)
        //                {
        //                    bodyBuilder.Attachments.Add(attachment.FileName, Convert.FromBase64String(attachment.DownloadUrl));
        //                }
        //            }

        //            message.Body = bodyBuilder.ToMessageBody();

        //            await client.SendAsync(message);
        //            await client.DisconnectAsync(true);
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error forwarding email: {ex.Message}");
        //        }
        //    }
        //}

        public async Task SendReplyAsync1(string to, string cc, string bcc, string subject, string body, string inReplyTo, List<IFormFile> attachments)
        {
            using (var smtpClient = new SmtpClient())
            {
                try
                {
                    // Connect to SMTP
                    await smtpClient.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                    await smtpClient.AuthenticateAsync(_emailAddress, _emailPassword);

                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress(_emailAddress, _emailAddress)); // Set your email
                    message.To.AddRange(to.Split(";").Select(x => new MailboxAddress(x, x.Trim())));

                    if (!string.IsNullOrWhiteSpace(cc))
                        message.Cc.AddRange(cc.Split(";").Select(x => new MailboxAddress(x, x.Trim())));

                    if (!string.IsNullOrWhiteSpace(bcc))
                        message.Bcc.AddRange(bcc.Split(";").Select(x => new MailboxAddress(x, x.Trim())));

                    message.Subject = $"Re: {subject}";
                    message.InReplyTo = inReplyTo;
                    message.References.Add(inReplyTo);

                    var bodyBuilder = new BodyBuilder { HtmlBody = body };

                    // Add Attachments
                    foreach (var file in attachments)
                    {
                        using var stream = new MemoryStream();
                        await file.CopyToAsync(stream);
                        bodyBuilder.Attachments.Add(file.FileName, stream.ToArray(), ContentType.Parse(file.ContentType));
                    }

                    message.Body = bodyBuilder.ToMessageBody();

                    // Send Email
                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }

        public async Task SendReplyAsyncv(string to, string cc, string bcc, string subject,string body, string messageId, List<AttachmentData> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", "your@email.com"));
            message.To.Add(new MailboxAddress(to, to));

            if (!string.IsNullOrEmpty(cc))
                message.Cc.Add(new MailboxAddress(cc, cc));

            if (!string.IsNullOrEmpty(bcc))
                message.Bcc.Add(new MailboxAddress(bcc, bcc));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { TextBody = body };

            // Attach files
            foreach (var attachment in attachments)
            {
                bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
            }

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.your-email.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync("your@email.com", "yourpassword");
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        public async Task SendReplyAsync(string to, string cc, string bcc,      string subject, string body, string messageId,      List<AttachmentData> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", _emailAddress));
            message.To.Add(new MailboxAddress(to, to));

            if (!string.IsNullOrEmpty(cc))
                message.Cc.Add(new MailboxAddress(cc, cc));

            if (!string.IsNullOrEmpty(bcc))
                message.Bcc.Add(new MailboxAddress(bcc, bcc));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { TextBody = body };

            // Attach files
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // Set In-Reply-To header for email threading
            if (!string.IsNullOrEmpty(messageId))
            {
                message.Headers.Add("In-Reply-To", messageId);
                message.Headers.Add("References", messageId);
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

    }

}
