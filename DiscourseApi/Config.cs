using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace BaseLibrary
{
    public abstract class Config : ConfigSection
    {
        public abstract IEnumerable<ConfigSection> ConfigSections { get; }

        public override void Load(string filename)
        {
            foreach (var configSection in ConfigSections)
            {
                configSection.Load(filename);
            }

            base.Load(filename);
        }

        public override void Load(XElement root)
        {
            foreach (var configSection in ConfigSections)
            {
                configSection.Load(root);
            }

            base.Load(root);
        }
    }

    public abstract class ConfigSection
    {
        private const string ModuleTag = "Module";

        public List<XElement> Modules { get; private set; }

        public abstract IEnumerable<ConfigItem> ConfigItems { get; }

        public ConfigSection()
        {
            Modules = new List<XElement>();
        }

        public virtual void Load(string filename)
        {
            var document = XDocument.Load(filename);
            var root = document.Root;
            Load(root);
        }

        public virtual void Load(XElement root)
        {
            foreach (var configItem in ConfigItems)
            {
                configItem.Load(root);
            }

            foreach (var moduleElement in root.Elements(ModuleTag))
            {
                Modules.Add(moduleElement); 
            }
        }
    }

    public abstract class ConfigItem
    {
        public abstract void Load(XElement root);
    }

    public abstract class ConfigItem<T> : ConfigItem
    {
        protected string Tag { get; private set; }
        private Action<T> _assign;

        public ConfigItem(string tag, Action<T> assign)
        {
            Tag = tag;
            _assign = assign;
        }

        protected abstract T Convert(string value);

        public override void Load(XElement root)
        {
            var elements = root.Elements(Tag);

            if (!elements.Any())
            {
                throw new XmlException("Config node " + Tag + " not found");
            }
            else if (elements.Count() >= 2)
            {
                throw new XmlException("Config node " + Tag + " ambigous");
            }

            _assign(Convert(elements.Single().Value));
        }
    }

    public class ConfigItemString : ConfigItem<string>
    {
        public ConfigItemString(string tag, Action<string> assign)
            : base(tag, assign)
        {
        }

        protected override string Convert(string value)
        {
            return value;
        }
    }

    public class ConfigItemInt32 : ConfigItem<int>
    {
        public ConfigItemInt32(string tag, Action<int> assign) 
            : base(tag, assign)
        {
        }

        protected override int Convert(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            else
            {
                throw new XmlException("Cannot convert value of config node " + Tag + " to integer"); 
            }
        }
    }

    public class ConfigItemBool : ConfigItem<bool>
    {
        public ConfigItemBool(string tag, Action<bool> assign)
            : base(tag, assign)
        {
        }

        protected override bool Convert(string value)
        {
            switch (value.ToLowerInvariant())
            {
                case "yes":
                case "true":
                case "1":
                    return true;
                default:
                    return false; 
            }
        }
    }

    public class ConfigItemBytes : ConfigItem<byte[]>
    {
        public ConfigItemBytes(string tag, Action<byte[]> assign) 
            : base(tag, assign)
        {
        }

        protected override byte[] Convert(string value)
        {
            var bytes = value.TryParseHexBytes();

            if (bytes != null)
            {
                return bytes;
            }
            else
            {
                throw new XmlException("Cannot convert value of config node " + Tag + " to bytes"); 
            }
        }
    }
}
