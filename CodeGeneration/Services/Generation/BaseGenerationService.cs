using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation
{
    public abstract class BaseGenerationService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        protected readonly IDictionary<string, IDictionary<string, string>> ServiceCache;
        private readonly IRazorTemplateService _razorTemplateService;
        
        protected BaseGenerationService(IRazorTemplateService razorTemplateService, [CallerFilePath]string callerFile = "")
        {
            Logger.Info("Creating new instance of BaseGenerationService from caller '{0}'", callerFile);

            ServiceCache = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            _razorTemplateService = razorTemplateService;
        }

        public IDictionary<string, IDictionary<string, string>> GetCache()
        {
            return ServiceCache;
        }

        public string GetCachedResult(string modelName, string templateName)
        {
            if (!ServiceCache.ContainsKey(modelName)) return string.Empty;

            var modelTemplates = ServiceCache[modelName];
            if (!modelTemplates.ContainsKey(templateName)) return string.Empty;

            return modelTemplates[templateName];
        }

        public string GetTemplate(string templateName)
        {
            return _razorTemplateService.ResolveTemplate(templateName);
        }

        protected static string GetTemplateNameFromEmbeddedResource(string resource)
        {
            var parts = resource.Split('.');
            return string.Join(".", parts.Skip(1).Take(parts.Length - 2));
        }

        protected static string GetFileNameFromEmbeddedResource(string resource)
        {
            var parts = resource.Split('.');
            var fileName = $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}";

            return fileName;
        }

        protected static string GetViewNameFromEmbeddedResource(string resource)
        {
            var parts = resource.Split('.');
            return parts[parts.Length - 2];
        }
    }
}
