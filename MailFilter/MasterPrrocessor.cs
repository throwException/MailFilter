using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MimeKit;
using MailKit;
using MailKit.Net;
using MailKit.Net.Imap;

namespace MailFilter
{
    public class MasterProcessor : Processor
    {
        private readonly MailFilterConfig _config;
        private readonly Fetcher _fetcher;

        public MasterProcessor(Context context, MailFilterConfig config)
            : base(context, config)
        {
            _config = config;
            _fetcher = new Fetcher(Context.Imap);
        }

        public void ProcessOne()
        {
            var m = _fetcher.GetNext();

            if (m != null)
            {
                Process(m);
            }
            else
            {
                Thread.Sleep(200);
            }
        }

        private void Process(IMessageSummary summary)
        {
            Process(summary, null);
        }
    }
}
