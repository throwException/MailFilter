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
    public class AddTagModuleConfig : Config
    {
        public string TagName { get; set; }

        public AddTagModuleConfig()
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
            }
        }
    }

    public class AddTagModule : Module
    {
        private AddTagModuleConfig _config;

        public AddTagModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new AddTagModuleConfig();
            _config.Load(configXml);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            var tags = new HashSet<string>();
            tags.Add(_config.TagName);
            Context.Imap.Inbox.AddFlags(summary.UniqueId, MessageFlags.None, tags, false);

            return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
        }
    }

    public class AddTagModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "AddTag"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new AddTagModule(context, configXml);
        }
    }
}
