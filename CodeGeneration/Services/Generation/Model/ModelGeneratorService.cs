using System;
using System.Collections.Generic;
using System.Linq;
using CodeGeneration.Extensions;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.Model
{
    public class ModelGeneratorService : BaseGenerationService, IModelGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITableMetadataService _tableMetadataService;
        private readonly IRazorTemplateService _razorTemplateService;

        public ModelGeneratorService(ITableMetadataService tableMetadataService, IRazorTemplateService razorTemplateService) 
            : base(razorTemplateService)
        {
            _tableMetadataService = tableMetadataService;
            _razorTemplateService = razorTemplateService;
        }

        public void Generate(ModelGenerationContext context)
        {
            var options = context.AppSettings.ModelGeneration;
            var connectionKey = context.AppSettings.SourceConnectionKey;
            var database = context.AppSettings.SourceDatabase;
            var schema = context.AppSettings.SourceSchema;

            var tableMetadata = _tableMetadataService.GetTableMetadata(connectionKey, database, schema);

            foreach (var table in tableMetadata)
            {
                var modelName = table.TableName.ToCamelCase();
                Logger.Info("Model Name: {0}", modelName);

                if (!ServiceCache.ContainsKey(modelName)) ServiceCache.Add(modelName, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                var modelCache = ServiceCache[modelName];

                var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories);
                if(options.TemplateNames.Any()) embeddedResources = embeddedResources.Where(er => options.TemplateNames.Any(er.Contains));

                foreach (var resource in embeddedResources)
                {
                    var razorEngineKey = GetTemplateNameFromEmbeddedResource(resource);
                    var fileName = GetFileNameFromEmbeddedResource(resource);
                    var viewName = GetViewNameFromEmbeddedResource(resource);

                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("File Name: {0}", fileName);
                    Logger.Info("Template Name: {0}", viewName);

                    var parsedCode = _razorTemplateService.Process(razorEngineKey, table);
                    Logger.Info(parsedCode);

                    if (modelCache.ContainsKey(viewName))
                        modelCache[viewName] = parsedCode;
                    else
                        modelCache.Add(viewName, parsedCode);
                }
            }
        }
    }
}
