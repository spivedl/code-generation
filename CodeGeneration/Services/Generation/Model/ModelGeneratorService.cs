using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeGeneration.Extensions;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.Template.Razor;
using NLog;

namespace CodeGeneration.Services.Generation.Model
{
    public class ModelGeneratorService : IModelGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string TemplateDirectory = "ModelGenerator";
        private const string TemplateName = "Model.cshtml";

        private readonly ITableMetadataService _tableMetadataService;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly IDictionary<string, IDictionary<string, string>> _modelCache;

        public ModelGeneratorService(ITableMetadataService tableMetadataService, IRazorTemplateService razorTemplateService)
        {
            _tableMetadataService = tableMetadataService;
            _razorTemplateService = razorTemplateService;

            _modelCache = new Dictionary<string, IDictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        }

        public void Generate(ModelGenerationContext context)
        {
            var connectionKey = "keeptime";
            var database = context.AppSettings.SqlGeneration.SourceDatabase;
            var schema = context.AppSettings.SqlGeneration.SourceSchema;

            var tableMetadata = _tableMetadataService.GetTableMetadata(connectionKey, database, schema);
            var assembly = Assembly.GetExecutingAssembly();

            foreach (var metadata in tableMetadata)
            {
                var modelName = metadata.TableName.ToCamelCase();
                Logger.Info("Model Name: {0}", modelName);

                if (!_modelCache.ContainsKey(modelName)) _modelCache.Add(modelName, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase));
                var cache = _modelCache[modelName];

                foreach (var resource in assembly.GetManifestResourceNames())
                {
                    if (!resource.Contains(TemplateDirectory)) continue;
                    if (!resource.Contains(TemplateName)) continue;

                    var razorEngineKey = GetTemplateNameFromEmbeddedResource(resource);
                    var fileName = GetFileNameFromEmbeddedResource(resource);
                    var templateName = GetViewNameFromEmbeddedResource(resource);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("File Name: {0}", fileName);
                    Logger.Info("Template Name: {0}", templateName);

                    var modelCode = _razorTemplateService.Process(razorEngineKey, metadata);
                    Logger.Info(modelCode);

                    if (cache.ContainsKey(templateName))
                        cache[templateName] = modelCode;
                    else
                        cache.Add(templateName, modelCode);
                }
            }
        }

        // TODO: Add a base generator service that implements the Get method because I think this logic will be same no matter the type of template (model, sql, etc.)
        public string Get(string modelName, string templateName)
        {
            if (!_modelCache.ContainsKey(modelName)) return string.Empty;

            var cache = _modelCache[modelName];

            if (!cache.ContainsKey(templateName)) return string.Empty;

            return cache[templateName];
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
