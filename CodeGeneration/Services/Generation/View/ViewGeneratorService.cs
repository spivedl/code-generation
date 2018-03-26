using System.Linq;
using System.Reflection;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Models.Metadata;
using CodeGeneration.Models.Metadata.Model;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.View
{
    public class ViewGeneratorService : IViewGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRazorTemplateService _razorTemplateService;

        public ViewGeneratorService(IRazorTemplateService razorTemplateService)
        {
            _razorTemplateService = razorTemplateService;
        }

        public void Generate(ViewGenerationContext context)
        {
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var resource in assembly.GetManifestResourceNames())
            {
                var razorTemplateName = GetTemplateNameFromEmbeddedResource(resource);
                var fileName = GetFileNameFromEmbeddedResource(resource);
                var templateName = GetViewNameFromEmbeddedResource(resource);
                Logger.Info("Embedded resource: {0}", resource);
                Logger.Info("Template Name: {0}", razorTemplateName);
                Logger.Info("File Name: {0}", fileName);
                Logger.Info("View Name: {0}", templateName);

                var model = new ModelMetadata(templateName, typeof(AppSettings));
                var content = _razorTemplateService.Process(razorTemplateName, model);

                Logger.Info(content);
            }
        }

        public string Get(string modelName, string templateName)
        {
            throw new System.NotImplementedException();
        }

        private static string GetTemplateNameFromEmbeddedResource(string resource)
        {
            var parts = resource.Split('.');
            return string.Join(".", parts.Skip(1).Take(parts.Length - 2));
        }

        private static string GetFileNameFromEmbeddedResource(string resource)
        {
            var parts = resource.Split('.');
            var fileName = $"{parts[parts.Length - 2]}.{parts[parts.Length - 1]}";

            return fileName;
        }

        private static string GetViewNameFromEmbeddedResource(string resource)
        {
            var parts = resource.Split('.');
            return parts[parts.Length - 2];
        }
    }
}