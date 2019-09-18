using System;
using MailKit;
using MailKit.Net;
using MailKit.Net.Imap;

namespace MailFilter
{
    public class Master
    {
        private readonly MailFilterConfig _config;
        private readonly ImapClient _imap;
        private readonly MasterProcessor _processor;
        private readonly Logger _logger;

        public Master(string configFileName)
        {
            _config = new MailFilterConfig();
            _config.Load(configFileName);
            _logger = new Logger(_config.LogFilePrefix);
            _logger.Info("Mail Filter programm started.");

            _imap = new ImapClient();
            _imap.Connect(_config.Imap.ImapServerHost, _config.Imap.ImapServerPort, true);
            _logger.Info("IMAP connected.");
            _imap.Authenticate(_config.Imap.MailAccountName, _config.Imap.MailAccountPassword);
            _logger.Info("IMAP autenticated.");
            _imap.Inbox.Open(FolderAccess.ReadWrite);
            _logger.Notice("IMAP inbox folder opened.");

            var context = new Context(_imap, _logger);
            _processor = new MasterProcessor(context, _config);
        }

        public void Run()
        {
            while (true)
            {
                _processor.ProcessOne();
            }
        }
    }
}
