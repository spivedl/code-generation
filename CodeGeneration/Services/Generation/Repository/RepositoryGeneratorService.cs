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

namespace CodeGeneration.Services.Generation.Repository
{
    public class RepositoryGeneratorService : IRepositoryGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ICacheService _cacheService;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly ICompilerService _compilerService;
        private readonly IFileWriter _fileWriter;

        public RepositoryGeneratorService(ICacheService cacheService, IRazorTemplateService razorTemplateService, IModelGeneratorService modelGeneratorService, ICompilerService compilerService, IFileWriter fileWriter)
        {
            _razorTemplateService = razorTemplateService;
            _modelGeneratorService = modelGeneratorService;
            _compilerService = compilerService;
            _fileWriter = fileWriter;
            _cacheService = cacheService;
        }

        public void Generate(RepositoryGenerationContext context)
        {
            if (!context.ApplicationOptions.GenerateRepositories)
            {
                Logger.Info("Repository generation is disabled. Change the 'GenerateRepositories' option in the 'appsettings.json' file to enable.");
                return;
            }

            var options = context.ApplicationOptions.RepositoryGeneration;

            var modelCache = _modelGeneratorService.GetCache();
            var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories, options.TemplateNames);

            foreach (var resource in embeddedResources)
            {
                var razorEngineKey = resource.ToRazorEngineKey();
                var templateName = resource.ToTemplateName();

                foreach (var modelCacheKvp in modelCache)
                {
                    var domainModelName = modelCacheKvp.Key;
                    var domainModelSource = modelCacheKvp.Value;
                    var cacheKey = _cacheService.BuildCacheKey(domainModelName, templateName);

                    Logger.Info("Model Name: {0}", domainModelName);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("Template Name: {0}", templateName);

                    var domainModelType = _compilerService.GetCompiledTypeFromSource(domainModelName, domainModelSource);
                    var templateModel = new ViewModelMetadata(templateName, domainModelType);

                    string parsedCode;
                    if (_cacheService.Exists(cacheKey))
                    {
                        Logger.Info("[CACHE HIT]: Repository code for {0} found in cache.", cacheKey);
                        parsedCode = _cacheService.Get<string>(cacheKey);
                    }
                    else
                    {
                        Logger.Info("[CACHE MISS]: Repository code for {0} NOT found in cache. Will process template and add to cache.", cacheKey);

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
            var className = $"{modelName}{templateName}";
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, className), extension);

            Logger.Info("Writing generated REPOSITORY to output file at '{0}'.", fullPath);
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