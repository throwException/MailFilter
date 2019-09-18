using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using MailKit;
using MailKit.Net;
using MailKit.Net.Smtp;
using MimeKit;
using BaseLibrary;

namespace MailFilter
{
    public class Mailer
    {
        private const string ErrorSubject = "Error in O2A";
        private const string WarningSubject = "Warning in O2A";
        private Logger _log;
        private ConfigSectionSmtp _config;

        public Mailer(Logger log, ConfigSectionSmtp config)
        {
            _log = log;
            _config = config;
        }

        public void Send(string to, string subject, string plainBody)
        {
            _log.Verbose("Sending message to {0}", to);

            try
            {
                var client = new SmtpClient();
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.Connect(_config.SmtpServerHost, _config.SmtpServerPort);
                client.Authenticate(_config.MailAccountName, _config.MailAccountPassword);
                _log.Verbose("Connected to mail server {0}:{1}", _config.SmtpServerHost, _config.SmtpServerPort);

                var text = new TextPart("plain") { Text = plainBody };
                text.ContentTransferEncoding = ContentEncoding.QuotedPrintable;

                var message = new MimeMessage();
                message.From.Add(InternetAddress.Parse(_config.SystemMailAddress));
                message.To.Add(InternetAddress.Parse(to));
                message.Subject = subject;
                message.Body = text;
                client.Send(message);

                _log.Info("Message sent to {0}", to);
            }
            catch (Exception exception)
            {
                _log.Error("Error sending mail to {0}", to);
                _log.Error(exception.ToString());
            }
        }

        public void Send(InternetAddress to, string subject, Multipart content)
        {
            Send(new MailboxAddress(_config.SystemMailAddress), to, subject, content);
        }

        public void Send(InternetAddress from, InternetAddress to, string subject, Multipart content)
        {
            Send(Create(from, to, subject, content));
        }

        public void Send(MimeMessage message, params MailboxAddress[] tos)
        {
            _log.Verbose("Sending message to {0}", string.Join(", ", tos.Select(a => a.Address)));

            try
            {
                var client = new SmtpClient();
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.Connect(_config.SmtpServerHost, _config.SmtpServerPort);
                client.Authenticate(_config.MailAccountName, _config.MailAccountPassword);
                _log.Verbose("Connected to mail server {0}:{1}", _config.SmtpServerHost, _config.SmtpServerPort);

                client.Send(message, new MailboxAddress(_config.SystemMailAddress), tos);
                _log.Info("Message sent to {0}", string.Join(", ", tos.Select(a => a.Address)));
            }
            catch (Exception exception)
            {
                _log.Error("Error sending mail to {0}", string.Join(", ", tos.Select(a => a.Address)));
                _log.Error(exception.ToString());
                throw exception;
            }
        }

        public void Send(MimeMessage message)
        {
            _log.Verbose("Sending message to {0}", message.To[0]);

            try
            {
                var client = new SmtpClient();
                client.SslProtocols = System.Security.Authentication.SslProtocols.None;
                client.Connect(_config.SmtpServerHost, _config.SmtpServerPort);
                client.Authenticate(_config.MailAccountName, _config.MailAccountPassword);
                _log.Verbose("Connected to mail server {0}:{1}", _config.SmtpServerHost, _config.SmtpServerPort);

                client.Send(message);
                _log.Info("Message sent to {0}", message.To[0]);
            }
            catch (Exception exception)
            {
                _log.Error("Error sending mail to {0}", message.To[0]);
                _log.Error(exception.ToString());
                throw exception;
            }
        }

        public MimeMessage Create(InternetAddress from, InternetAddress to, string subject, Multipart content)
        {
            var message = new MimeMessage();
            message.From.Add(from);
            message.To.Add(to);
            message.Subject = subject;
            message.Body = content;

            return message;
        }
    }
}
