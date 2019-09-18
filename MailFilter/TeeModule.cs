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
    public class TeeModuleConfig : Config
    {
        public TeeModuleConfig()
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

    public class TeeModule : Module
    {
        private readonly TeeModuleConfig _config;
        private readonly Processor _processor;

        public TeeModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new TeeModuleConfig();
            _config.Load(configXml);
            _processor = new Processor(context, _config);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            _processor.Process(summary, message); 

            return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
        }
    }

    public class TeeModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "Tee"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new TeeModule(context, configXml);
        }
    }
}
