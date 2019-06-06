using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using MailKit.Net.Smtp;
using MimeKit;

namespace Ace.Utility
{
    public class EmailUtil
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="model">邮件基础信息</param>
        /// <param name="sendServer">发件人基础信息</param>
        public static void SendEmail(EmailBodyModel model, SendServerConfiguration sendServer)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(model.FromAddress));
            message.To.Add(new MailboxAddress(model.ToAddress));

            message.Subject = model.Subject;

            Multipart multipart = new Multipart("mixed");

            if (!string.IsNullOrEmpty(model.Body))
            {
                multipart.Add(new MultipartAlternative()
                {
                    model.IsHtmlBody
                        ? new BodyBuilder {HtmlBody = model.Body}.ToMessageBody()
                        : new TextPart("plain") {Text = model.Body}
                });
            }

            if (model.BccUserEmail != null && model.BccUserEmail.Any())
            {
                List<MailboxAddress> mailboxAddresses = new List<MailboxAddress>();
                model.BccUserEmail.ForEach(item => mailboxAddresses.Add(new MailboxAddress(item)));
                message.Bcc.AddRange(mailboxAddresses);
            }

            if (model.CcUserEmail != null && model.CcUserEmail.Any())
            {
                List<MailboxAddress> mailboxAddresses = new List<MailboxAddress>();
                model.CcUserEmail.ForEach(item => mailboxAddresses.Add(new MailboxAddress(item)));
                message.Cc.AddRange(mailboxAddresses);
            }

            if (model.Attachments != null && model.Attachments.Any())
            {
                model.Attachments.ForEach(item =>
                {
                    multipart.Add(new MimePart()
                    {
                        Content = new MimeContent(File.OpenRead(item)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(item)
                    });
                });

            }

            message.Body = multipart;

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(sendServer.SmtpHost, sendServer.SmtpPort, sendServer.IsSsl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(sendServer.SenderAccount, sendServer.SenderPassword);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }

    /// <summary>
    /// 邮件内容实体
    /// </summary>
    public class EmailBodyModel
    {
        /// <summary>
        /// 邮件的发件人地址
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// 邮件收件人的地址
        /// </summary>
        public string ToAddress { get; set; }

        /// <summary>
        /// 电子邮件的主题行
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件正文是否是HTML
        /// </summary>
        public bool IsHtmlBody { get; set; }

        /// <summary>
        /// 邮件正文
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 邮件密件抄送 (BCC) 收件人的地址集合
        /// </summary>
        public List<string> BccUserEmail { get; set; }

        /// <summary>
        /// 邮件抄送 (CC) 收件人的地址集合
        /// </summary>
        public List<string> CcUserEmail { get; set; }

        /// <summary>
        /// 发送内容格式 默认utf8
        /// </summary>
        public Encoding BodyEncoding { get; set; } = System.Text.Encoding.UTF8;

        /// <summary>
        /// 附件地址
        /// </summary>
        public List<string> Attachments { get; set; }
    }

    /// <summary>
    /// 邮件发送服务器配置
    /// </summary>
    public class SendServerConfiguration
    {
        /// <summary>
        /// 邮箱SMTP服务器地址
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// 邮箱SMTP服务器端口
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// 是否启用IsSsl
        /// </summary>
        public bool IsSsl { get; set; }

        /// <summary>
        /// 邮件编码
        /// </summary>
        public string MailEncoding { get; set; }

        /// <summary>
        /// 发件人账号
        /// </summary>
        public string SenderAccount { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string SenderPassword { get; set; }
    }
}
