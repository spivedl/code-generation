using System.Collections.Generic;
using System.IO;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Metadata.Model;
using CodeGeneration.Services.Cache;
using CodeGeneration.Services.Compiler;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.View
{
    public class ViewGeneratorService : IViewGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICacheService _cacheService;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly ICompilerService _compilerService;
        private readonly IFileWriter _fileWriter;

        public ViewGeneratorService(ICacheService cacheService, IRazorTemplateService razorTemplateService, IModelGeneratorService modelGeneratorService, ICompilerService compilerService, IFileWriter fileWriter)
        {
            _razorTemplateService = razorTemplateService;
            _modelGeneratorService = modelGeneratorService;
            _compilerService = compilerService;
            _fileWriter = fileWriter;
            _cacheService = cacheService;
        }

        public void Generate(ViewGenerationContext context)
        {
            var options = context.ApplicationOptions.ViewGeneration;

            foreach (var modelCache in _modelGeneratorService.GetCache())
            {
                var domainModelName = modelCache.Key;
                var domainModelSource = modelCache.Value;
                var embeddedTemplates = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories, options.TemplateNames);

                foreach (var resource in embeddedTemplates)
                {
                    var razorEngineKey = resource.ToRazorEngineKey();
                    var fileName = resource.ToFileName();
                    var templateName = resource.ToTemplateName();
                    var cacheKey = _cacheService.BuildCacheKey(domainModelName, templateName);

                    Logger.Info("Model Name: {0}", domainModelName);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("File Name: {0}", fileName);
                    Logger.Info("Template Name: {0}", templateName);

                    var domainModelType = _compilerService.GetCompiledTypeFromSource(domainModelName, domainModelSource);
                    var templateModel = new ModelMetadata(templateName, domainModelType);

                    string parsedCode;
                    if (_cacheService.Exists(cacheKey))
                    {
                        Logger.Info("[CACHE HIT]: Model code for {0} found in cache.", cacheKey);
                        parsedCode = _cacheService.Get<string>(cacheKey);
                    }
                    else
                    {
                        Logger.Info("[CACHE MISS]: Model code for {0} NOT found in cache. Will process template and add to cache.", cacheKey);

                        parsedCode = _razorTemplateService.Process(razorEngineKey, templateModel);
                        _cacheService.Set(cacheKey, parsedCode);
                    }

                    if (options.Output.GenerateOutput) WriteToFile(options.Output, domainModelName, templateName, parsedCode);
                }
            }
        }

        private void WriteToFile(OutputOptions options, string modelName, string templateName, string contents)
        {
            var basePath = options.Path;
            var extension = options.Extension;
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, modelName, templateName), extension);

            Logger.Info("Writing generated VIEW to output file at '{0}'.", fullPath);
            _fileWriter.WriteAllText(fullPath, contents);
        }

        public IDictionary<string, string> GetCache()
        {
            return _cacheService.Get<string>();
        }

        public string GetCachedResult(string modelName = "", string templateName = "")
        {
            return _cacheService.Get<string>(_cacheService.BuildCacheKey(modelName, templateName));
        }

        public string GetTemplate(string templateName, string templateDirectory = "")
        {
            return _razorTemplateService.ResolveTemplate(templateName.ToRazorEngineKey(templateDirectory));
        }
    }
}