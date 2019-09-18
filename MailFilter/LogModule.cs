using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Text.RegularExpressions;
using BaseLibrary;
using MimeKit;
using MailKit;
using MailKit.Net;
using MailKit.Net.Imap;

namespace MailFilter
{
    public class LogModuleConfig : Config
    {
        public string Text { get; set; }
        public LogSeverity Severity { get; set; }

        public LogModuleConfig()
        {
        }

        public override IEnumerable<ConfigSection> ConfigSections
        {
            get
            {
                return new ConfigSection[0];
            }
        }

        public override IEnumerable<ConfigItem> ConfigItems
        {
            get
            {
                yield return new ConfigItemString("Text", v => Text = v);
                yield return new ConfigItemString("Severity", v => Severity = (LogSeverity)Enum.Parse(typeof(LogSeverity), v));
            }
        }
    }

    public class LogModule : Module
    {
        private LogModuleConfig _config;

        public LogModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new LogModuleConfig();
            _config.Load(configXml);
        }

        public string Format(MimeMessage message)
        {
            var text = _config.Text;

            if (message != null)
            {
                text = text.Replace("{subject}", message.Subject);
                text = text.Replace("{from}", string.Join(", ", message.From
                    .Select(a => (MailboxAddress)a)
                    .Select(a => string.Format("{0} <{1}>", a.Name, a.Address))));
                text = text.Replace("{to}", string.Join(", ", message.To
                    .Select(a => (MailboxAddress)a)
                    .Select(a => string.Format("{0} <{1}>", a.Name, a.Address))));
                text = text.Replace("{cc}", string.Join(", ", message.Cc
                    .Select(a => (MailboxAddress)a)
                    .Select(a => string.Format("{0} <{1}>", a.Name, a.Address))));
            }

            return text;
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            Context.Logger.Write(_config.Severity, Format(message));
            return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
        }
    }

    public class LogModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "Log"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new LogModule(context, configXml);
        }
    }
}
