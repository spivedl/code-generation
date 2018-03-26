using System;
using NLog;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace CodeGeneration.Services.Template.Razor
{
    public class RazorTemplateService : IRazorTemplateService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRazorEngineService _razorEngine;

        public RazorTemplateService()
        {
            ITemplateManager templateManager = new EmbeddedResourceTemplateManager(typeof(Program));
            ITemplateServiceConfiguration razorConfiguration = new TemplateServiceConfiguration
            {
                TemplateManager = templateManager,
                CachingProvider = new DefaultCachingProvider(t => { })
            };
            _razorEngine = RazorEngineService.Create(razorConfiguration);
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