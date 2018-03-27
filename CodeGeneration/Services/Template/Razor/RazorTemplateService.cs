using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NLog;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace CodeGeneration.Services.Template.Razor
{
    // TODO: Add logging to the RazorTemplateService
    public class RazorTemplateService : IRazorTemplateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();

        private readonly ITemplateManager _templateManager;
        private readonly IRazorEngineService _razorEngine;

        public RazorTemplateService()
        {
            _templateManager = new EmbeddedResourceTemplateManager(typeof(Program));
            ITemplateServiceConfiguration razorConfiguration = new TemplateServiceConfiguration
            {
                TemplateManager = _templateManager,
                CachingProvider = new DefaultCachingProvider(t => { })
            };

            _razorEngine = RazorEngineService.Create(razorConfiguration);
        }

        public IEnumerable<string> GetEmbeddedTemplateNames()
        {
            return _assembly.GetManifestResourceNames();
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> paths)
        {
            var names = new List<string>();

            foreach (var path in paths)
            {
                names.AddRange(GetEmbeddedTemplateNames(path));
            }

            return names;
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(string path)
        {
            return _assembly.GetManifestResourceNames().Where(rn => rn.Contains(path));
        }

        public string ResolveTemplate(string templateName)
        {
            try
            {
                var templateKey = _razorEngine.GetKey(templateName);
                var templateSource = _templateManager.Resolve(templateKey);

                return templateSource.Template;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error resolving template for name '{templateName}'!");
                return string.Empty;
            }
        }

        public string Process(string templateName, object model = null, Type modelType = null)
        {
            return _razorEngine.RunCompile(templateName, modelType, model);
        }

        public string Process(string templateName, Type modelType = null, object model = null, DynamicViewBag viewBag = null)
        {
            return _razorEngine.RunCompile(templateName, modelType, model, viewBag);
        }
    }
}