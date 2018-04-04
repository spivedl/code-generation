using System.Collections.Generic;
using System.IO;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Services.Cache;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Process;
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
        private readonly ISqlCommandExecutorService _sqlCommandExecutor;

        public SqlGeneratorService(ITableMetadataService tableMetadataService, IRazorTemplateService razorTemplateService, ICacheService cacheService, IFileWriter fileWriter, ISqlCommandExecutorService sqlCommandExecutor) 
        {
            _tableMetadataService = tableMetadataService;
            _razorTemplateService = razorTemplateService;
            _cacheService = cacheService;
            _fileWriter = fileWriter;
            _sqlCommandExecutor = sqlCommandExecutor;
        }

        public void Generate(SqlGenerationContext context)
        {
            if (!context.ApplicationOptions.GenerateSql)
            {
                Logger.Info("SQL generation is disabled. Change the 'GenerateSql' option in the 'appsettings.json' file to enable.");
                return;
            }

            var options = context.ApplicationOptions.SqlGeneration;
            var connectionKey = context.ApplicationOptions.SourceConnectionKey;
            var database = context.ApplicationOptions.SourceDatabase;
            var schema = context.ApplicationOptions.SourceSchema;
            var readOnlyColumns = context.ApplicationOptions.ReadOnlyProperties;

            var tableMetadata = _tableMetadataService.GetTableMetadata(new TableMetadataContext(connectionKey, database, schema, readOnlyColumns));
            var embeddedResources = _razorTemplateService.GetEmbeddedTemplateNames(options.TemplateDirectories, options.TemplateNames);

            foreach (var resource in embeddedResources)
            {
                var outputPath = string.Empty;
                var razorEngineKey = resource.ToRazorEngineKey();
                var templateName = resource.ToTemplateName();

                foreach (var table in tableMetadata)
                {
                    var modelName = table.TableName.ToCamelCase();
                    var cacheKey = _cacheService.BuildCacheKey(modelName, templateName);

                    Logger.Info("Model Name: {0}", modelName);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Razor Engine Key: {0}", razorEngineKey);
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

                    if (!options.Output.GenerateOutput) continue;

                    outputPath = WriteToFile(options.Output, templateName, parsedCode);
                }

                if (!string.IsNullOrWhiteSpace(outputPath) && options.SqlCommand.ExecuteSql) _sqlCommandExecutor.ExecuteSqlCommand(outputPath, options.SqlCommand);
            }
        }

        private string WriteToFile(OutputOptions options, string templateName, string contents)
        {
            var basePath = options.Path;
            var extension = options.Extension;
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, templateName), extension);

            Logger.Info("Writing generated SQL to output file at '{0}'.", fullPath);
            _fileWriter.WriteLines(fullPath, contents, append: true);

            return fullPath;
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
