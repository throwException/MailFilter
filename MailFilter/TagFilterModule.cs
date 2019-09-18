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
    public class TagFilterModuleConfig : Config
    {
        public string TagName { get; private set; }
        public bool Negate { get; private set; }

        public TagFilterModuleConfig()
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
                yield return new ConfigItemString("TagName", v => TagName = v);
                yield return new ConfigItemBool("Negate", v => Negate = v);
            }
        }
    }

    public class TagFilterModule : Module
    {
        private TagFilterModuleConfig _config;

        public TagFilterModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new TagFilterModuleConfig();
            _config.Load(configXml);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            if (_config.Negate ^ summary.Keywords.Contains(_config.TagName))
            {
                return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
            }
            else
            {
                return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Stop, message);
            }
        }
    }

    public class TagFilterModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "TagFilter"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new TagFilterModule(context, configXml);
        }
    }
}
