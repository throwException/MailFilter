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
    public class DayTimeFilterModuleConfig : Config
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public DayTimeFilterModuleConfig()
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
                yield return new ConfigItemString("StartTime", v => StartTime = TimeSpan.Parse(v));
                yield return new ConfigItemString("EndTime", v => EndTime = TimeSpan.Parse(v));
            }
        }
    }

    public class DayTimeFilterModule : Module
    {
        private DayTimeFilterModuleConfig _config;

        public DayTimeFilterModule(Context context, XElement configXml)
            : base(context, configXml)
        {
            _config = new DayTimeFilterModuleConfig();
            _config.Load(configXml);
        }

        private bool InRange(DateTimeOffset date)
        {
            var time = date.LocalDateTime.Subtract(date.LocalDateTime.Date);

            if (_config.StartTime >= _config.EndTime)
            {
                return time >= _config.StartTime && time <= _config.EndTime;
            }
            else
            {
                return time >= _config.StartTime || time <= _config.EndTime;
            }
        }

        public override Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message)
        {
            if (InRange(message.Date))
            {
                return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Continue, message);
            }
            else
            {
                return new Tuple<ModuleResult, MimeMessage>(ModuleResult.Stop, message);
            }
        }
    }

    public class DayTimeFilterModuleFactory : ModuleFactory
    {
        public override string Name
        {
            get { return "DayTimeFilter"; } 
        }

        public override Module Create(Context context, XElement configXml)
        {
            return new DayTimeFilterModule(context, configXml);
        }
    }
}
