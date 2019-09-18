using System;
using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net;
using MailKit.Net.Imap;

namespace MailFilter
{
    public class Fetcher
    {
        private readonly ImapClient _imap;
        private readonly Queue<IMessageSummary> _queue;
        private ulong _modSeq = 0;

        public Fetcher(ImapClient imap)
        {
            _imap = imap;
            _queue = new Queue<IMessageSummary>();

            var idList = _imap.Inbox.Fetch(0, -1, _modSeq, MessageSummaryItems.UniqueId | MessageSummaryItems.Fast);
            foreach (var m in idList)
            {
                if (m.ModSeq.HasValue && m.ModSeq.Value > _modSeq)
                {
                    _modSeq = m.ModSeq.Value;
                }

                _queue.Enqueue(m);
            }
        }

        public IMessageSummary GetNext()
        {
            if (_queue.Count > 0)
            {
                return _queue.Dequeue();
            }
            else
            {
                var newList = _imap.Inbox.Fetch(0, -1, _modSeq, MessageSummaryItems.UniqueId | MessageSummaryItems.Fast);
                foreach (var m in newList)
                {
                    if (m.ModSeq.HasValue && m.ModSeq.Value > _modSeq)
                    {
                        _modSeq = m.ModSeq.Value;
                    }

                    _queue.Enqueue(m);
                }

                if (_queue.Count > 0)
                {
                    return _queue.Dequeue();
                }
                else
                {
                    return null; 
                }
            }
        }
    }
}
