using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using MimeKit;
using MailKit;
using MailKit.Net;
using MailKit.Net.Imap;

namespace MailFilter
{
    public abstract class ModuleFactory
    {
        public abstract string Name { get; }

        public abstract Module Create(Context context, XElement configXml);
    }

    public enum ModuleResult
    {
        Continue,
        Stop 
    }

    public abstract class Module
    {
        protected Context Context { get; private set; }

        public Module(Context context, XElement configXml)
        {
            Context = context;
        }

        public abstract Tuple<ModuleResult, MimeMessage> Execute(IMessageSummary summary, MimeMessage message);
    }
}
