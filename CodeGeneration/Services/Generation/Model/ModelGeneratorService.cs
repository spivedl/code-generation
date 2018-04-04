using System.Collections.Generic;
using System.IO;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Services.Cache;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.Model
{
    public class ModelGeneratorService : IModelGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITableMetadataService _tableMetadataService;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly ICacheService _cacheService;
        private readonly IFileWriter _fileWriter;

        public ModelGeneratorService(ITableMetadataService tableMetadataService, IRazorTemplateService razorTemplateService, ICacheService cacheService, IFileWriter fileWriter) 
        {
            _tableMetadataService = tableMetadataService;
            _razorTemplateService = razorTemplateService;
            _cacheService = cacheService;
            _fileWriter = fileWriter;
        }

        public void Generate(ModelGenerationContext context)
        {
            if (!context.ApplicationOptions.GenerateModels)
            {
                Logger.Info("Model generation is disabled. Change the 'GenerateModels' option in the 'appsettings.json' file to enable.");
                return;
            }

            var options = context.ApplicationOptions.ModelGeneration;
            var connectionKey = context.ApplicationOptions.SourceConnectionKey;
            var database = context.ApplicationOptions.SourceDatabase;
            var schema = context.ApplicationOptions.SourceSchema;
            var readOnlyColumns = context.ApplicationOptions.ReadOnlyProperties;

            var tableMetadata = _tableMetadataService.GetTableMetadata(new TableMetadataContext(connectionKey, database, schema, readOnlyColumns));
            var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories, options.TemplateNames);

            foreach (var resource in embeddedResources)
            {
                foreach (var table in tableMetadata)
                {
                    var modelName = table.TableName.ToCamelCase();
                    var razorEngineKey = resource.ToRazorEngineKey();
                    var templateName = resource.ToTemplateName();

                    Logger.Info("Model Name: {0}", modelName);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("Template Name: {0}", templateName);

                    string parsedCode;
                    if (_cacheService.Exists(modelName))
                    {
                        Logger.Info("[CACHE HIT]: Model code for {0} found in cache.", modelName);
                        parsedCode = _cacheService.Get<string>(modelName);
                    }
                    else
                    {
                        Logger.Info("[CACHE MISS]: Model code for {0} NOT found in cache. Will process template and add to cache.", modelName);

                        parsedCode = _razorTemplateService.Process(razorEngineKey, table);
                        _cacheService.Set(modelName, parsedCode);
                    }

                    if (options.Output.GenerateOutput) WriteToFile(options.Output, modelName, parsedCode);
                }
            }
        }

        private void WriteToFile(OutputOptions options, string modelName, string contents)
        {
            var basePath = options.Path;
            var extension = options.Extension;
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, modelName), extension);

            Logger.Info("Writing generated MODEL to output file at '{0}'.", fullPath);
            _fileWriter.WriteAllText(fullPath, contents);
        }

        public IDictionary<string, string> GetCache()
        {
            return _cacheService.Get<string>();
        }

        public string GetCachedResult(string modelName = "", string templateName = "")
        {
            return _cacheService.Get<string>(modelName);
        }

        public string GetTemplate(string templateName, string templateDirectory = "")
        {
            return _razorTemplateService.ResolveTemplate(templateName.ToRazorEngineKey(templateDirectory));
        }
    }
}
