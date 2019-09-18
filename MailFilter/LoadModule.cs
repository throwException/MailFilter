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
    public class LoadModuleConfig : Config
    {
        public LoadModuleConfig()
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
                return new ConfigItem[0];
            }
        }
    }

    public class LoadModule : Module
    {
        private LoadModuleConfig _config;

        public LoadModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new LoadModuleConfig();
            _config.Load(configXml);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            message = Context.Imap.Inbox.GetMessage(summary.UniqueId);

            return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
        }
    }

    public class LoadModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "Load"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new LoadModule(context, configXml);
        }
    }
}
