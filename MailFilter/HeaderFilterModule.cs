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
    public class HeaderFilterModuleConfig : Config
    {
        public string Field { get; set; }
        public string ValuePattern { get; set; }

        public HeaderFilterModuleConfig()
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
                yield return new ConfigItemString("Field", v => Field = v);
                yield return new ConfigItemString("ValuePattern", v => ValuePattern = v);
            }
        }
    }

    public class HeaderFilterModule : Module
    {
        private HeaderFilterModuleConfig _config;

        public HeaderFilterModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new HeaderFilterModuleConfig();
            _config.Load(configXml);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            if (message.Headers
                .Any(h => h.Field == _config.Field && Regex.IsMatch(h.Value, _config.ValuePattern)))
            {
                return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
            }
            else
            {
                return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Stop, message);
            }
        }
    }

    public class HeaderFilterModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "HeaderFilter"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new HeaderFilterModule(context, configXml);
        }
    }
}
