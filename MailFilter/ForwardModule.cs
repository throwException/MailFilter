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
    public class ForwardModuleConfig : Config
    {
        public ConfigSectionSmtp Smtp { get; private set; }
        public string Recipient { get; private set; }

        public ForwardModuleConfig()
        {
            Smtp = new ConfigSectionSmtp();
        }

        public override IEnumerable<ConfigSection> ConfigSections
        {
            get
            {
                yield return Smtp;
            }
        }

        public override IEnumerable<ConfigItem> ConfigItems
        {
            get
            {
                yield return new ConfigItemString("Recipient", v => Recipient = v);
            }
        }
    }

    public class ForwardModule : Module
    {
        private readonly ForwardModuleConfig _config;
        private readonly Mailer _mailer;

        public ForwardModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new ForwardModuleConfig();
            _config.Load(configXml);
            _mailer = new Mailer(Context.Logger, _config.Smtp);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            _mailer.Send(message, new MailboxAddress(_config.Recipient));

            return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
        }
    }

    public class ForwardModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "Forward"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new ForwardModule(context, configXml);
        }
    }
}
