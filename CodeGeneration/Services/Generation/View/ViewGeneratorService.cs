using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Models.Metadata.Model;
using CodeGeneration.Services.Compiler;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.View
{
    public class ViewGeneratorService : BaseGenerationService, IViewGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly ICompilerService _compilerService;

        public ViewGeneratorService(IRazorTemplateService razorTemplateService, IModelGeneratorService modelGeneratorService, ICompilerService compilerService) : base(razorTemplateService)
        {
            _razorTemplateService = razorTemplateService;
            _modelGeneratorService = modelGeneratorService;
            _compilerService = compilerService;
        }

        public void Generate(ViewGenerationContext context)
        {
            var options = context.AppSettings.ViewGeneration;

            foreach (var modelCache in _modelGeneratorService.GetCache())
            {
                var modelName = modelCache.Key;
                var viewCache = modelCache.Value.First(mc => mc.Key.Equals("Model"));
                var modelSource = viewCache.Value;

                if (!ServiceCache.ContainsKey(modelName)) ServiceCache.Add(modelName, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));

                var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories);
                if (options.TemplateNames.Any()) embeddedResources = embeddedResources.Where(er => options.TemplateNames.Any(er.Contains));

                foreach (var resource in embeddedResources)
                {
                    var razorEngineKey = GetTemplateNameFromEmbeddedResource(resource);
                    var fileName = GetFileNameFromEmbeddedResource(resource);
                    var templateName = GetViewNameFromEmbeddedResource(resource);

                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("File Name: {0}", fileName);
                    Logger.Info("Template Name: {0}", templateName);

                    var compilerResults = _compilerService.CompileAssemblyFromSource(modelSource);
                    var model = new ModelMetadata(templateName, compilerResults.CompiledAssembly.ExportedTypes.First());
                    var parsedCode = _razorTemplateService.Process(razorEngineKey, model);
                    Logger.Info(parsedCode);

                    if (ServiceCache[modelName].ContainsKey(templateName))
                        ServiceCache[modelName][templateName] = parsedCode;
                    else
                        ServiceCache[modelName].Add(templateName, parsedCode);
                }
            }
        }
    }
}