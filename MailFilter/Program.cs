using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using MailKit;
using MailKit.Net;
using MailKit.Net.Imap;

namespace MailFilter
{
    public static class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("usage: mono MailFilter.exe <configfilename>");
                return;
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Config file {0} not found.", args[0]);
                return;
            }

            var master = new Master(args[0]);
            master.Run();
        }
    }
}
