using System;
using MailKit.Net.Imap;

namespace MailFilter
{
    public class Context
    {
        public ImapClient Imap { get; private set; }
        public Logger Logger { get; private set; }

        public Context(ImapClient imap, Logger logger)
        {
            Imap = imap;
            Logger = logger;
        }
    }
}
