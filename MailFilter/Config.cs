using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using BaseLibrary;

namespace MailFilter
{
    public class ConfigSectionImap : ConfigSection
    {
        public string ImapServerHost { get; set; }
        public int ImapServerPort { get; set; }
        public string MailAccountName { get; set; }
        public string MailAccountPassword { get; set; }

        public override IEnumerable<ConfigItem> ConfigItems
        {
            get
            {
                yield return new ConfigItemString("ImapServerHost", v => ImapServerHost = v);
                yield return new ConfigItemInt32("ImapServerPort", v => ImapServerPort = v);
                yield return new ConfigItemString("MailAccountName", v => MailAccountName = v);
                yield return new ConfigItemString("MailAccountPassword", v => MailAccountPassword = v);
            } 
        }
    }

    public class ConfigSectionSmtp : ConfigSection
    {
        public string SmtpServerHost { get; set; }
        public int SmtpServerPort { get; set; }
        public string MailAccountName { get; set; }
        public string MailAccountPassword { get; set; }
        public string SystemMailAddress { get; private set; }
        public string SystemMailName { get; private set; }

        public override IEnumerable<ConfigItem> ConfigItems
        {
            get
            {
                yield return new ConfigItemString("SmtpServerHost", v => SmtpServerHost = v);
                yield return new ConfigItemInt32("SmtpServerPort", v => SmtpServerPort = v);
                yield return new ConfigItemString("MailAccountName", v => MailAccountName = v);
                yield return new ConfigItemString("MailAccountPassword", v => MailAccountPassword = v);
                yield return new ConfigItemString("SystemMailAddress", v => SystemMailAddress = v);
                yield return new ConfigItemString("SystemMailName", v => SystemMailName = v);
            }
        }
    }

    public class MailFilterConfig : Config
    {
        public ConfigSectionImap Imap { get; private set; }

        public string LogFilePrefix { get; set; }

        public MailFilterConfig()
        {
            Imap = new ConfigSectionImap(); 
        }

        public override IEnumerable<ConfigSection> ConfigSections
        {
            get
            {
                yield return Imap;
            }
        }

        public override IEnumerable<ConfigItem> ConfigItems
        {
            get
            {
                yield return new ConfigItemString("LogFilePrefix", v => LogFilePrefix = v);
            }
        }
    }
}
