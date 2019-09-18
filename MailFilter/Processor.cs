using System.Collections.Generic;
using System.Linq;
using BaseLibrary;
using MailKit;
using MimeKit;

namespace MailFilter
{
    public class Processor
    {
        private const string ModuleNameAttribute = "name";
        private readonly List<Module> _filters;

        protected Context Context { get; private set; }

        public Processor(Context context, ConfigSection config)
        {
            Context = context;
            _filters = new List<Module>();

            foreach (var moduleElement in config.Modules)
            {
                if (moduleElement.Attributes().Any(a => a.Name == ModuleNameAttribute))
                {
                    var name = moduleElement.Attribute(ModuleNameAttribute).Value;
                    var factory = FilterFactories.Single(f => f.Name == name);
                    var filter = factory.Create(context, moduleElement);
                    _filters.Add(filter);
                }
                else
                {
                    Context.Logger.Warning("Module element without name attribute."); 
                }
            }
        }

        private IEnumerable<ModuleFactory> FilterFactories
        {
            get 
            {
                yield return new HeaderFilterModuleFactory();
                yield return new LogModuleFactory();
                yield return new TeeModuleFactory();
                yield return new AddTagModuleFactory();
                yield return new LoadModuleFactory();
                yield return new TagFilterModuleFactory();
                yield return new PostDiscourseModuleFactory();
                yield return new ForwardModuleFactory();
            }
        }

        public void Process(IMessageSummary summary, MimeMessage message)
        {
            foreach (var filter in _filters)
            {
                var result = filter.Execute(summary, message);

                if (result.Item1 == ModuleResult.Stop)
                {
                    return;
                }
                else
                {
                    message = result.Item2; 
                }
            }
        }
    }
}
