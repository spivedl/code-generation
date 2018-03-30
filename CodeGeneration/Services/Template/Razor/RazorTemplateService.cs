using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private readonly IEnumerable<string> _embeddedResources;

        private readonly ITemplateManager _templateManager;
        private readonly IRazorEngineService _razorEngine;

        public RazorTemplateService()
        {
            _embeddedResources = _assembly.GetManifestResourceNames();

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
            return _embeddedResources;
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> filterByPaths)
        {
            return _embeddedResources.Where(er => filterByPaths.Any(er.Contains));
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(IEnumerable<string> filterByPaths, IEnumerable<string> filterByNames)
        {
            var embeddedResourcesByPaths = _embeddedResources.Where(er => filterByPaths.Any(er.Contains));

            return embeddedResourcesByPaths.Where(er => filterByNames.Any(er.Contains));
        }

        public IEnumerable<string> GetEmbeddedTemplateNames(string filterByPath)
        {
            return _embeddedResources.Where(er => er.Contains(filterByPath));
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