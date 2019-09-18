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
using DiscourseApi;

namespace MailFilter
{
    public class PostDiscourseModuleConfig : Config
    {
        public DiscourseApiConfig Discourse { get; private set; }
        public int TopicId { get; private set; }

        public PostDiscourseModuleConfig()
        {
            Discourse = new DiscourseApiConfig();
        }

        public override IEnumerable<ConfigSection> ConfigSections
        {
            get
            {
                yield return Discourse;
            }
        }

        public override IEnumerable<ConfigItem> ConfigItems
        {
            get
            {
                yield return new ConfigItemInt32("TopicId", v => TopicId = v);
            }
        }
    }

    public class PostDiscourseModule : Module
    {
        private readonly PostDiscourseModuleConfig _config;
        private readonly Discourse _discourse;

        public PostDiscourseModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new PostDiscourseModuleConfig();
            _config.Load(configXml);
            _discourse = new Discourse(_config.Discourse);
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            try
            {
                var topic = _discourse.GetTopic(_config.TopicId);
                _discourse.Post(topic, message.TextBody);
            }
            catch (Exception exception)
            {
                Context.Logger.Error("Could not post on discourse\n" + exception.ToString());
            }

            return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
        }
    }

    public class PostDiscourseModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "PostDiscourse"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new PostDiscourseModule(context, configXml);
        }
    }
}
