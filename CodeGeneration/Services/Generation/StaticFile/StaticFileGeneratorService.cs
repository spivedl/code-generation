﻿using System.Collections.Generic;
using System.IO;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Template;
using CodeGeneration.Services.Cache;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.StaticFile
{
    public class StaticFileGeneratorService : IStaticFileGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITableMetadataService _tableMetadataService;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly ICacheService _cacheService;
        private readonly IFileWriter _fileWriter;

        public StaticFileGeneratorService(ITableMetadataService tableMetadataService, IRazorTemplateService razorTemplateService, ICacheService cacheService, IFileWriter fileWriter)
        {
            _tableMetadataService = tableMetadataService;
            _razorTemplateService = razorTemplateService;
            _cacheService = cacheService;
            _fileWriter = fileWriter;
        }

        public void Generate(GenerationContext context)
        {
            if (!context.ApplicationOptions.GenerateStaticFiles)
            {
                Logger.Info("Static File generation is disabled. Change the 'GenerateStaticFile' option in the 'appsettings.json' file to enable.");
                return;
            }

            var options = context.ApplicationOptions.StaticFileGeneration;
            var tableMetadataSet = _tableMetadataService.GetTableMetadata(new TableMetadataContext(context.ApplicationOptions));
            var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories, options.TemplateNames);

            foreach (var resource in embeddedResources)
            {
                var razorEngineKey = resource.ToRazorEngineKey();
                var templateName = resource.ToTemplateName();
                var cacheKey = _cacheService.BuildCacheKey(templateName);

                Logger.Info("Embedded resource: {0}", resource);
                Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                Logger.Info("Template Name: {0}", templateName);

                string parsedContents;
                if (_cacheService.Exists(cacheKey))
                {
                    Logger.Info("[CACHE HIT]: Static File code for {0} found in cache.", cacheKey);
                    parsedContents = _cacheService.Get<string>(cacheKey);
                }
                else
                {
                    Logger.Info("[CACHE MISS]: Static File code for {0} NOT found in cache. Will process template and add to cache.", cacheKey);

                    parsedContents = _razorTemplateService.Process(razorEngineKey, new StaticFileTemplateModel { TargetNamespace = options.Namespace, TableMetadataSet = tableMetadataSet });
                    _cacheService.Set(cacheKey, parsedContents);
                }

                if (options.Output.GenerateOutput) WriteToFile(options.Output, templateName, parsedContents);
            }
        }

        private void WriteToFile(OutputOptions options, string templateName, string contents)
        {
            var basePath = options.Path;
            var extension = options.Extension;
            var fileName = $"{templateName}";
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, fileName), extension);

            Logger.Info("Writing generated Static File to output file at '{0}'.", fullPath);
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