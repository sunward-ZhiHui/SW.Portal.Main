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
using System.Text.RegularExpressions;
using Core.Repositories.Query;
using Microsoft.Extensions.Options;

namespace AC.SD.Core.Services
{
    public class OutlookEmailService
    {
        private readonly string _smtpServer;       
        private readonly string _imapServer;
        //private readonly string _imapServer = "mail.sunwardpharma.com";
        private readonly int _imapPort;
        private readonly int _smtpPort;
        //private readonly int _imapPort = 143;
        private readonly AttachmentService _attachmentService;
        private readonly string _emailAddress = "demoinfotech001@gmail.com";
        private readonly string _emailPassword = "ruke ncnc qfbh jahu";

        private readonly IApplicationUserQueryRepository _applicationUserQueryRepository;
        public OutlookEmailService(IOptions<OutlookEmailSettings> emailSettings,AttachmentService attachmentService, IApplicationUserQueryRepository applicationUserQueryRepository)
        {

            _smtpServer = emailSettings.Value.SmtpServer;
            _imapServer = emailSettings.Value.ImapServer;
            _imapPort = emailSettings.Value.ImapPort;
            _smtpPort = emailSettings.Value.SmtpPort;

            _attachmentService = attachmentService;
            _applicationUserQueryRepository = applicationUserQueryRepository;
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

        public async Task<List<EmailModel>> ReceiveEmailsAsync19_03()
        {
            var emails = new List<EmailModel>();
            var emailDict = new Dictionary<string, EmailModel>();
            var orphanReplies = new List<EmailModel>();

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected.");

                    await client.AuthenticateAsync(_emailAddress, _emailPassword);
                    Console.WriteLine("Authenticated successfully.");

                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    var results = await inbox.SearchAsync(SearchQuery.All);

                    var tempEmailList = new List<MimeMessage>();

                    foreach (var uid in results)
                    {
                        var message = await inbox.GetMessageAsync(uid);
                        tempEmailList.Add(message);
                    }

                    tempEmailList = tempEmailList.OrderBy(m => m.Date).ToList();

                    foreach (var message in tempEmailList)
                    {
                        var messageId = message.MessageId;
                        var inReplyTo = message.InReplyTo;
                        var references = message.References?.ToArray();

                        if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
                        {
                            inReplyTo = references.Last();
                        }

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
                            Attachments = attachments,
                            IsAttachment = attachments.Any(),
                            Replies = new List<EmailReply>()
                        };

                        emailDict[messageId] = email;
                    }

                    Console.WriteLine($"Total emails fetched: {emailDict.Count}");

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

                    Console.WriteLine($"Orphan replies found: {orphanReplies.Count}");

                    foreach (var email in orphanReplies)
                    {
                        if (!string.IsNullOrEmpty(email.InReplyTo))
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

                                if (!possibleParent.IsAttachment && email.IsAttachment)
                                {
                                    possibleParent.Attachments.AddRange(email.Attachments);
                                    possibleParent.IsAttachment = true;
                                }
                            }
                        }
                    }

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

                            if (!mainEmail.IsAttachment && email.IsAttachment)
                            {
                                mainEmail.Attachments.AddRange(email.Attachments);
                                mainEmail.IsAttachment = true;
                            }
                        }
                    }

                    emails = groupedEmails.Values.OrderByDescending(d => d.Date).ToList();
                    Console.WriteLine($"Total grouped emails: {emails.Count}");

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return emails;
        }

        public async Task<bool> LoginAuthentication(string emailAddress, string emailPassword)
        {
            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected.");

                    await client.AuthenticateAsync(emailAddress, emailPassword);
                    Console.WriteLine("Authenticated successfully.");

                    await client.DisconnectAsync(true);
                    return true; // Login successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Login failed: {ex.Message}");
                    return false; // Login failed
                }
            }
        }

        public async Task<List<EmailModel>> ReceiveEmailsAsync22()
        {
            var uniqueInboxEmails = new Dictionary<string, EmailModel>(); // Main list (Only 1 email per subject)
            var allEmails = new Dictionary<string, EmailModel>(); // Store all emails for replies

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected.");

                    await client.AuthenticateAsync(_emailAddress, _emailPassword);
                    Console.WriteLine("Authenticated successfully.");

                    // Step 1: Fetch Emails from Inbox (Main List)
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    await FetchEmailsFromFolder(inbox, uniqueInboxEmails, allEmails, true);

                    // Step 2: Fetch Emails from Sent (Only for Replies)
                    var sentFolder = await GetSentFolderAsync(client);
                    if (sentFolder != null)
                    {
                        await FetchEmailsFromFolder(sentFolder, null, allEmails, false);
                    }

                    Console.WriteLine($"Total emails fetched: {allEmails.Count}");

                    // Step 3: Group Replies under Unique Inbox Emails (Sorted by Date)
                    foreach (var email in allEmails.Values)
                    {
                        var normalizedSubject = NormalizeSubject(email.Subject);

                        if (uniqueInboxEmails.TryGetValue(normalizedSubject, out var parentEmail))
                        {
                            parentEmail.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                BodyHtml = email.BodyHtml,
                                Date = email.Date.DateTime,
                                From = email.FromName,
                                Attachments = email.Attachments,
                                IsToMe = email.To.Contains(_emailAddress),
                                IsSentByMe = email.From.Contains(_emailAddress)
                            });

                            //  Ensure Replies are Ordered by Date (Oldest First)
                            parentEmail.Replies = parentEmail.Replies.OrderBy(r => r.Date).ToList();
                        }
                    }



                    Console.WriteLine($"Total Unique Inbox Emails (with Replies): {uniqueInboxEmails.Count}");

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return uniqueInboxEmails.Values.OrderByDescending(e => e.Date).ToList();
        }
        public async Task<List<EmailModel>> ReceiveEmailsAsync(long userId)
        {
            var userData = await _applicationUserQueryRepository.GetByApplicationUsersList(userId);
            var checkLogin = await LoginAuthentication(userData.OutlookEmailID, userData.OutlookPassword);

            var uniqueInboxEmails = new Dictionary<string, EmailModel>(); // Declare before login check
            var allEmails = new Dictionary<string, EmailModel>(); // Store all emails for replies

            if (!checkLogin)
            {
                uniqueInboxEmails.Add("LoginFailed", new EmailModel
                {
                    Subject = "Login Failed",
                    BodyHtml = "Failed to authenticate with the email server. Please check your credentials.",
                    FromName = "System",
                    Date = DateTime.Now
                });

                return uniqueInboxEmails.Values.ToList(); // Convert to List<EmailModel> before returning
            }

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected.");

                    await client.AuthenticateAsync(userData.OutlookEmailID, userData.OutlookPassword);
                    Console.WriteLine("Authenticated successfully.");

                    // Step 1: Fetch Emails from Inbox (Main List)
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    await FetchEmailsFromFolder(inbox, uniqueInboxEmails, allEmails, true);

                    // Step 2: Fetch Emails from Sent (Only for Replies)
                    var sentFolder = await GetSentFolderAsync(client);
                    if (sentFolder != null)
                    {
                        await FetchEmailsFromFolder(sentFolder, null, allEmails, false);
                    }

                    Console.WriteLine($"Total emails fetched: {allEmails.Count}");

                    // Step 3: Group Replies under Unique Inbox Emails (Sorted by Date)
                    foreach (var email in allEmails.Values)
                    {
                        var normalizedSubject = NormalizeSubject(email.Subject);

                        if (uniqueInboxEmails.TryGetValue(normalizedSubject, out var parentEmail))
                        {
                            parentEmail.Replies.Add(new EmailReply
                            {
                                Body = email.BodyText,
                                BodyHtml = email.BodyHtml,
                                Date = email.Date.DateTime,
                                From = email.FromName,
                                Attachments = email.Attachments,
                                IsToMe = email.To.Contains(userData.OutlookEmailID), // Fixed
                                IsSentByMe = email.From.Contains(userData.OutlookEmailID) // Fixed
                            });

                            // Ensure Replies are Ordered by Date (Oldest First)
                            parentEmail.Replies = parentEmail.Replies.OrderBy(r => r.Date).ToList();
                        }
                    }

                    Console.WriteLine($"Total Unique Inbox Emails (with Replies): {uniqueInboxEmails.Count}");

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return uniqueInboxEmails.Values.OrderByDescending(e => e.Date).ToList();
        }


        public async Task<List<EmailModel>> ReceiveEmailsByUserAsync(long userId)
        {
            var userData = await _applicationUserQueryRepository.GetByApplicationUsersList(userId);
            var checkLogin = await LoginAuthentication(userData.OutlookEmailID,userData.OutlookPassword);

            var emails = new List<EmailModel>();

            if (!checkLogin)
            {
                emails.Add(new EmailModel
                {
                    Subject = "Login Failed",
                    BodyHtml = "Failed to authenticate with the email server. Please check your credentials.",
                    FromName = "System",
                    Date = DateTime.Now
                });
                return emails;
            }

            return emails;
        }

        private async Task FetchEmailsFromFolder(IMailFolder folder, Dictionary<string, EmailModel> mainDict, Dictionary<string, EmailModel> allEmailsDict, bool isInbox)
        {
            //var todayStart = DateTime.UtcNow.Date; // Start of the current day in UTC
            //var results = await folder.SearchAsync(SearchQuery.DeliveredAfter(todayStart));

            var results = await folder.SearchAsync(SearchQuery.All);
            foreach (var uid in results)
            {
                var message = await folder.GetMessageAsync(uid);
                ProcessEmail(message, mainDict, allEmailsDict, isInbox);
            }
        }


        //private async Task<IMailFolder> GetSentFolderAsync(ImapClient client)
        //{
        //    var sentFolderNames = new[] { "Sent", "Sent Items", "[Gmail]/Sent Mail", "Sent Messages", "SENT-MAIL" };

        //    foreach (var ns in client.PersonalNamespaces)
        //    {
        //        var folders = await client.GetFoldersAsync(ns);
        //        var sentFolder = folders.FirstOrDefault(f => sentFolderNames.Contains(f.FullName, StringComparer.OrdinalIgnoreCase));
        //        if (sentFolder != null)
        //        {
        //            await sentFolder.OpenAsync(FolderAccess.ReadOnly);
        //            Console.WriteLine($"Sent folder '{sentFolder.FullName}' opened successfully.");
        //            return sentFolder;
        //        }
        //    }

        //    return null;
        //}

        private async Task ProcessEmail20(MimeMessage message, Dictionary<string, EmailModel> mainDict, Dictionary<string, EmailModel> allEmailsDict, bool isInbox)
        {
            var messageId = message.MessageId;
            var inReplyTo = message.InReplyTo;
            var references = message.References?.ToArray();

            if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
            {
                inReplyTo = references.Last();
            }

            var attachments = new List<EmailAttachmentModel>();
            var inlineImages = new Dictionary<string, string>();

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

                        // ✅ Store inline images separately
                        if (!string.IsNullOrEmpty(part.ContentId))
                        {
                            string cidKey = part.ContentId.Trim('<', '>');
                            inlineImages[cidKey] = downloadUrl;
                            Console.WriteLine($"Stored Inline Image: {cidKey} → {downloadUrl}");
                        }
                        else
                        {
                            // Regular attachment
                            attachments.Add(new EmailAttachmentModel
                            {
                                FileName = part.FileName,
                                ContentType = part.ContentType.MimeType,
                                DownloadUrl = downloadUrl
                            });
                        }
                    }
                }
            }

            // ✅ Debugging: Check if inline images were detected
            Console.WriteLine($"Inline Images Found: {inlineImages.Count}");


            // Step 2: Replace CID Images in BodyHtml
            string bodyHtml = message.HtmlBody ?? "";

            foreach (var (cid, url) in inlineImages)
            {
                // Ensure CID replacements happen correctly
                bodyHtml = bodyHtml.Replace($"cid:{cid}", url);
            }

            // Debugging: Print modified `BodyHtml`
            Console.WriteLine("Modified BodyHtml: " + bodyHtml);




            // ✅ Step 3: Create EmailModel Object
            var email = new EmailModel
            {
                Subject = message.Subject,
                BodyText = message.TextBody,
                BodyHtml = bodyHtml, // ✅ Updated with correct images
                From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                Date = message.Date.DateTime,
                MessageId = messageId,
                InReplyTo = inReplyTo,
                Attachments = attachments, // ✅ Attachments only for this email
                IsAttachment = attachments.Any(),
                Replies = new List<EmailReply>()
            };

            allEmailsDict[messageId] = email;

            // ✅ Step 4: Store Only Unique Subjects for Inbox Emails
            if (isInbox)
            {
                var normalizedSubject = NormalizeSubject(email.Subject);
                if (!mainDict.ContainsKey(normalizedSubject))
                {
                    mainDict[normalizedSubject] = email;
                }
            }
        }

        private async Task ProcessEmail(MimeMessage message, Dictionary<string, EmailModel> mainDict, Dictionary<string, EmailModel> allEmailsDict, bool isInbox)
        {
            var messageId = message.MessageId;
            var inReplyTo = message.InReplyTo;
            var references = message.References?.ToArray();

            if (string.IsNullOrEmpty(inReplyTo) && references != null && references.Length > 0)
            {
                inReplyTo = references.Last();
            }

            var attachments = new List<EmailAttachmentModel>();
            var inlineImages = new Dictionary<string, string>();

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

                        // ✅ Store inline images separately
                        if (!string.IsNullOrEmpty(part.ContentId))
                        {
                            string cidKey = part.ContentId.Trim('<', '>');
                            inlineImages[cidKey] = downloadUrl;
                            Console.WriteLine($"Stored Inline Image: {cidKey} → {downloadUrl}");
                        }
                        else
                        {
                            // Regular attachment
                            attachments.Add(new EmailAttachmentModel
                            {
                                FileName = part.FileName,
                                ContentType = part.ContentType.MimeType,
                                DownloadUrl = downloadUrl
                            });
                        }
                    }
                }
            }

            // 🔹 Step 1: Extract and Convert Inline Images (CID) to Base64         
            foreach (var part in message.BodyParts.OfType<MimePart>()) 
            {
                if (part.ContentId != null) 
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await part.Content.DecodeToAsync(memoryStream);
                        byte[] attachmentBytes = memoryStream.ToArray();
                        string base64String = Convert.ToBase64String(attachmentBytes);
                        string mimeType = part.ContentType.MimeType;

                        // 🔹 Convert CID Image to Base64 Data URL
                        string base64Image = $"data:{mimeType};base64,{base64String}";

                        // 🔹 Normalize CID Key
                        string cidKey = part.ContentId.Trim('<', '>');  // Remove `< >`
                        string cidSearch = $"cid:{cidKey}";  // Format used in email HTML

                        inlineImages[cidSearch] = base64Image;

                        Console.WriteLine($"✅ [CID] Stored Inline Image: {cidKey} → {base64Image.Substring(0, 50)}...");
                    }
                }
            }

            // 🔹 Step 2: Replace CID Images in the HTML Body
            string bodyHtml = message.HtmlBody ?? "";
            foreach (var (cid, base64Image) in inlineImages)
            {
                if (bodyHtml.Contains(cid))
                {
                    bodyHtml = bodyHtml.Replace(cid, base64Image);
                    Console.WriteLine($"✅ [CID] Replaced {cid} in BodyHtml.");
                }
            }

            // Debugging Output
            Console.WriteLine("📜 Final BodyHtml:\n" + bodyHtml);

            Console.WriteLine("Final Modified BodyHtml: " + bodyHtml); // Debugging Output

            // 🔹 Step 3: Create EmailModel Object
            var email = new EmailModel
            {
                Subject = message.Subject,
                BodyText = message.TextBody,
                BodyHtml = bodyHtml, // ✅ Fixed CID image replacement
                From = string.Join(";", message.From.Mailboxes.Select(m => m.Address)),
                To = string.Join(";", message.To.Mailboxes.Select(m => m.Address)),
                FromName = string.Join(",", message.From.Mailboxes.Select(m => m.Name)),
                ToName = string.Join(",", message.To.Mailboxes.Select(m => m.Name)),
                Date = message.Date.DateTime,
                MessageId = messageId,
                InReplyTo = inReplyTo,
                Attachments = attachments,
                IsAttachment = attachments.Any(),
                Replies = new List<EmailReply>()
            };

            allEmailsDict[messageId] = email;

            // 🔹 Step 4: Store Only Unique Subjects for Inbox Emails
            if (isInbox)
            {
                var normalizedSubject = NormalizeSubject(email.Subject);
                if (!mainDict.ContainsKey(normalizedSubject))
                {
                    mainDict[normalizedSubject] = email;
                }
            }
        }


        //private string NormalizeSubject(string subject)
        //{
        //    if (string.IsNullOrEmpty(subject))
        //        return subject;

        //    string normalized = subject.Trim();
        //    while (normalized.StartsWith("Re: ", StringComparison.OrdinalIgnoreCase) ||
        //           normalized.StartsWith("Fwd: ", StringComparison.OrdinalIgnoreCase))
        //    {
        //        normalized = normalized.Substring(4).Trim();
        //    }
        //    return normalized;
        //}




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

        public async Task SendReplyAsync21(string to, string cc, string bcc, string toName, string subject, string body, string messageId, List<AttachmentData> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(toName, _emailAddress));
            message.To.Add(new MailboxAddress(to, to));

            if (!string.IsNullOrEmpty(cc))
                message.Cc.Add(new MailboxAddress(cc, cc));

            if (!string.IsNullOrEmpty(bcc))
                message.Bcc.Add(new MailboxAddress(bcc, bcc));

            message.Subject = "Re: " + subject; // Prefixing with "Re:" for better email threading

            var bodyBuilder = new BodyBuilder { HtmlBody = body }; // Ensure it's HTML or Text

            // Attach files
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // Set In-Reply-To and References headers to maintain email thread
            if (!string.IsNullOrEmpty(messageId))
            {
                message.Headers.Add(HeaderId.InReplyTo, messageId);
                message.Headers.Add(HeaderId.References, messageId);
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendReplyAsync25(string to, string cc, string bcc, string toName, string subject, string body, string messageId, List<AttachmentData> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(toName, _emailAddress));
            message.To.Add(new MailboxAddress(to, to));

            if (!string.IsNullOrEmpty(cc))
                message.Cc.Add(new MailboxAddress(cc, cc));

            if (!string.IsNullOrEmpty(bcc))
                message.Bcc.Add(new MailboxAddress(bcc, bcc));

            message.Subject = "Re: " + subject;

            var bodyBuilder = new BodyBuilder();

            // Extract and attach inline images
            var inlineAttachments = new List<MimePart>();
            body = ExtractAndReplaceBase64Images(body, inlineAttachments);

            bodyBuilder.HtmlBody = body;

            // Add inline images as attachments
            foreach (var inlineAttachment in inlineAttachments)
            {
                bodyBuilder.LinkedResources.Add(inlineAttachment);
            }

            // Attach regular files
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // Set In-Reply-To and References headers
            if (!string.IsNullOrEmpty(messageId))
            {
                message.Headers.Add(HeaderId.InReplyTo, messageId);
                message.Headers.Add(HeaderId.References, messageId);
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendReplyAsync(string to, string cc, string bcc, string toName, string subject, string body, string messageId, List<AttachmentData> attachments)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(toName, _emailAddress));

            // Handle multiple recipients for To
            foreach (var email in to.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(new MailboxAddress(email.Trim(), email.Trim()));
            }

            // Handle multiple recipients for Cc
            if (!string.IsNullOrEmpty(cc))
            {
                foreach (var email in cc.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Cc.Add(new MailboxAddress(email.Trim(), email.Trim()));
                }
            }

            // Handle multiple recipients for Bcc
            if (!string.IsNullOrEmpty(bcc))
            {
                foreach (var email in bcc.Split(';', StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Bcc.Add(new MailboxAddress(email.Trim(), email.Trim()));
                }
            }

            message.Subject = "Re: " + subject;

            var bodyBuilder = new BodyBuilder();

            // Extract and attach inline images
            var inlineAttachments = new List<MimePart>();
            body = ExtractAndReplaceBase64Images(body, inlineAttachments);

            bodyBuilder.HtmlBody = body;

            // Add inline images as attachments
            foreach (var inlineAttachment in inlineAttachments)
            {
                bodyBuilder.LinkedResources.Add(inlineAttachment);
            }

            // Attach regular files
            if (attachments != null)
            {
                foreach (var attachment in attachments)
                {
                    bodyBuilder.Attachments.Add(attachment.FileName, attachment.Content, ContentType.Parse(attachment.ContentType));
                }
            }

            message.Body = bodyBuilder.ToMessageBody();

            // Set In-Reply-To and References headers
            if (!string.IsNullOrEmpty(messageId))
            {
                message.Headers.Add(HeaderId.InReplyTo, messageId);
                message.Headers.Add(HeaderId.References, messageId);
            }

            using var client = new SmtpClient();
            await client.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_emailAddress, _emailPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        private string ExtractAndReplaceBase64Images(string htmlBody, List<MimePart> inlineAttachments)
        {
            var matches = Regex.Matches(htmlBody, @"<img[^>]+src=""data:image/(?<type>[^;]+);base64,(?<data>[^""]+)""[^>]*>");

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var base64Data = match.Groups["data"].Value;
                    var imageType = match.Groups["type"].Value;
                    var imageData = Convert.FromBase64String(base64Data);

                    var imagePart = new MimePart("image", imageType)
                    {
                        Content = new MimeContent(new MemoryStream(imageData), ContentEncoding.Default),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Inline),
                        ContentId = Guid.NewGuid().ToString(),
                        FileName = $"image.{imageType}"
                    };

                    inlineAttachments.Add(imagePart);

                    // Replace the Base64 image in the HTML with a reference to the inline attachment
                    htmlBody = htmlBody.Replace(match.Value, $"<img src=\"cid:{imagePart.ContentId}\" alt=\"image\" />");
                }
            }

            return htmlBody;
        }


        public async Task<EmailModel> ReceiveEmailsReplyAsync(string messageId = null)
        {
            var allEmails = new Dictionary<string, EmailModel>(); // Store all emails from Inbox & Sent
            EmailModel filteredEmail = null;

            using (var client = new ImapClient())
            {
                try
                {
                    Console.WriteLine("Connecting to IMAP server...");
                    await client.ConnectAsync(_imapServer, _imapPort, SecureSocketOptions.SslOnConnect);
                    Console.WriteLine("Connected.");

                    await client.AuthenticateAsync(_emailAddress, _emailPassword);
                    Console.WriteLine("Authenticated successfully.");

                    // ✅ Step 1: Fetch Emails from Inbox
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(FolderAccess.ReadOnly);
                    await FetchEmailsFromFolderReply(inbox, allEmails, true);

                    // ✅ Step 2: Fetch Emails from Sent Folder
                    var sentFolder = await GetSentFolderAsync(client);
                    if (sentFolder != null)
                    {
                        await FetchEmailsFromFolderReply(sentFolder, allEmails, false);
                    }

                    Console.WriteLine($"Total emails fetched: {allEmails.Count}");

                    // ✅ Step 3: Filter by messageId (if provided)
                    if (!string.IsNullOrEmpty(messageId))
                    {
                        if (allEmails.TryGetValue(messageId, out var email))
                        {
                            filteredEmail = email;

                            // ✅ Find all related replies using InReplyTo and References
                            var relatedReplies = allEmails.Values
                                .Where(e => e.InReplyTo == messageId || e.References.Contains(messageId))
                                .OrderBy(e => e.Date)
                                .ToList();

                            // ✅ Populate Replies list
                            filteredEmail.Replies = relatedReplies.Select(e => new EmailReply
                            {
                                Body = e.BodyText,
                                BodyHtml = e.BodyHtml,
                                Date = e.Date.DateTime,
                                From = e.FromName,
                                Attachments = e.Attachments, // ✅ Include reply attachments
                                IsToMe = e.To.Contains(_emailAddress),
                                IsSentByMe = e.From.Contains(_emailAddress)
                            }).ToList();

                            Console.WriteLine($"Replies Found: {filteredEmail.Replies.Count}");
                        }
                        else
                        {
                            Console.WriteLine("Message ID not found.");
                            return null;
                        }
                    }

                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            return filteredEmail;
        }             
        private async Task FetchEmailsFromFolderReply(IMailFolder folder, Dictionary<string, EmailModel> allEmailsDict, bool isInbox)
        {
            var results = await folder.SearchAsync(SearchQuery.All);
            foreach (var uid in results)
            {
                var message = await folder.GetMessageAsync(uid);
                await ProcessEmailReply(message, allEmailsDict, isInbox);
            }
        }
        private async Task ProcessEmailReply(MimeMessage message, Dictionary<string, EmailModel> allEmailsDict, bool isInbox)
        {
            var messageId = message.MessageId;
            var inReplyTo = message.InReplyTo;
            var references = message.References?.ToArray() ?? Array.Empty<string>();

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

            // ✅ Create EmailModel
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
                //References = references.ToList(), // ✅ Store references for filtering replies
                Attachments = attachments, // ✅ Attachments included
                IsAttachment = attachments.Any(),
                Replies = new List<EmailReply>()
            };

            allEmailsDict[messageId] = email;
        }
              
        private async Task<IMailFolder> GetSentFolderAsync(ImapClient client)
        {
            var sentFolderNames = new[] { "Sent", "Sent Items", "[Gmail]/Sent Mail", "Sent Messages", "SENT-MAIL" };

            foreach (var ns in client.PersonalNamespaces)
            {
                var folders = await client.GetFoldersAsync(ns);
                var sentFolder = folders.FirstOrDefault(f => sentFolderNames.Contains(f.FullName, StringComparer.OrdinalIgnoreCase));
                if (sentFolder != null)
                {
                    await sentFolder.OpenAsync(FolderAccess.ReadOnly);
                    Console.WriteLine($"Sent folder '{sentFolder.FullName}' opened successfully.");
                    return sentFolder;
                }
            }

            return null;
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

    }

}
