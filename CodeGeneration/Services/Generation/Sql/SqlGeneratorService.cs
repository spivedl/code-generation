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

namespace CodeGeneration.Services.Generation.Sql
{
    public class SqlGeneratorService : ISqlGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITableMetadataService _tableMetadataService;
        private readonly IRazorTemplateService _razorTemplateService;
        private readonly ICacheService _cacheService;
        private readonly IFileWriter _fileWriter;

        public SqlGeneratorService(ITableMetadataService tableMetadataService, IRazorTemplateService razorTemplateService, ICacheService cacheService, IFileWriter fileWriter) 
        {
            _tableMetadataService = tableMetadataService;
            _razorTemplateService = razorTemplateService;
            _cacheService = cacheService;
            _fileWriter = fileWriter;
        }

        public void Generate(SqlGenerationContext context)
        {
            var options = context.ApplicationOptions.SqlGeneration;
            var connectionKey = context.ApplicationOptions.SourceConnectionKey;
            var database = context.ApplicationOptions.SourceDatabase;
            var schema = context.ApplicationOptions.SourceSchema;
            var readOnlyColumns = context.ApplicationOptions.ReadOnlyProperties;

            var tableMetadata = _tableMetadataService.GetTableMetadata(new TableMetadataContext(connectionKey, database, schema, readOnlyColumns));

            foreach (var table in tableMetadata)
            {
                var modelName = table.TableName.ToCamelCase();
                var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories, options.TemplateNames);

                foreach (var resource in embeddedResources)
                {
                    var razorEngineKey = resource.ToRazorEngineKey();
                    var fileName = resource.ToFileName();
                    var templateName = resource.ToTemplateName();
                    var cacheKey = _cacheService.BuildCacheKey(modelName, templateName);

                    Logger.Info("Model Name: {0}", modelName);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
                    Logger.Info("File Name: {0}", fileName);
                    Logger.Info("Template Name: {0}", templateName);

                    string parsedCode;
                    if (_cacheService.Exists(cacheKey))
                    {
                        Logger.Info("[CACHE HIT]: Sql code for {0} found in cache.", cacheKey);
                        parsedCode = _cacheService.Get<string>(cacheKey);
                    }
                    else
                    {
                        Logger.Info("[CACHE MISS]: Sql code for {0} NOT found in cache. Will process template and add to cache.", cacheKey);

                        parsedCode = _razorTemplateService.Process(razorEngineKey, table);
                        _cacheService.Set(cacheKey, parsedCode);
                    }

                    if(options.Output.GenerateOutput) WriteToFile(options.Output, modelName, templateName, parsedCode);
                }
            }
        }

        private void WriteToFile(OutputOptions options, string modelName, string templateName, string contents)
        {
            var basePath = options.Path;
            var extension = options.Extension;
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, modelName, templateName), extension);

            Logger.Info("Writing generated SQL to output file at '{0}'.", fullPath);
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
