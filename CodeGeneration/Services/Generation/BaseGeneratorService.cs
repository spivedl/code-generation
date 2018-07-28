using System;
using System.IO;
using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Template;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Template;
using NLog;

namespace CodeGeneration.Services.Generation
{
    public abstract class BaseGeneratorService : INewGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ITableMetadataService _tableMetadataService;
        private readonly ITemplateService _templateService;
        private readonly IFileWriter _fileWriter;

        protected BaseGeneratorService(ITableMetadataService tableMetadataService, ITemplateService templateService, IFileWriter fileWriter)
        {
            _tableMetadataService = tableMetadataService;
            _templateService = templateService;
            _fileWriter = fileWriter;
        }

        public virtual GenerationStatistics GenerateOutput(GenerationContext context)
        {
            if (!IsGeneratorEnabled(context)) return new GenerationStatistics { GenerationEnabled = false };

            var stats = new GenerationStatistics();

            var applicationOptions = context.ApplicationOptions;
            var generatorOptions = GetGeneratorOptions(context.ApplicationOptions);
            var connectionKey = context.ApplicationOptions.SourceConnectionKey;

            var tableMetadataSet = _tableMetadataService.GetTableMetadata(new TableMetadataContext(applicationOptions));
            var embeddedResources = _templateService.GetEmbeddedTemplateNames(generatorOptions.TemplateDirectories, generatorOptions.TemplateNames);

            foreach (var resource in embeddedResources)
            {
                foreach (var tableMetadata in tableMetadataSet)
                {
                    var entityName = tableMetadata.TableName.ToCamelCase();
                    var templateName = GetTemplateNameFromResource(resource);

                    // log information
                    Logger.Info("Table Name: {0}", tableMetadata.TableName);
                    Logger.Info("Entity Name: {0}", entityName);
                    Logger.Info("Embedded resource: {0}", resource);
                    Logger.Info("Template Name: {0}", templateName);
                    Logger.Info("Processing template for entity: {0}.", entityName);

                    // create model for template
                    var templateModel = GetTemplateModel(new TemplateModelContext
                    {
                        ConnectionKey = connectionKey,
                        TargetNamespace = generatorOptions.Namespace,
                        TableMetadata = tableMetadata
                    });

                    // process template
                    var parsedContents = _templateService.Process(resource, templateModel);

                    // update stats
                    stats.LinesOfCode += parsedContents.CountLines();
                    stats.NumFilesProcessed++;

                    // write output to file
                    if (!generatorOptions.Output.GenerateOutput) continue;
                    WriteToFile(new OutputContext
                    {
                        OutputOptions = generatorOptions.Output,
                        EntityName = entityName,
                        ResourceName = resource,
                        TemplateName = templateName,
                        TemplateContents = parsedContents
                    });

                    // update stats
                    stats.NumFilesOutput++;
                }
            }

            stats.EndTime = DateTime.UtcNow;
            return stats;
        }

        public virtual string GetTemplateContents(string templateName)
        {
            return _templateService.ResolveTemplate(templateName);
        }

        protected abstract bool IsGeneratorEnabled(GenerationContext context);

        protected abstract GenerationOptions GetGeneratorOptions(ApplicationOptions options);

        protected abstract TemplateModel GetTemplateModel(TemplateModelContext context);

        protected virtual string GetTemplateNameFromResource(string resourceName)
        {
            var parts = resourceName.Split('.');

            return parts.Length > 2
                ? parts[parts.Length - 2]
                : parts[0];
        }

        protected virtual void WriteToFile(OutputContext context)
        {
            var basePath = context.OutputOptions.Path;
            var extension = context.OutputOptions.Extension;
            var fileName = $"{context.TemplateName}".Replace("{{modelName}}", context.EntityName);
            var fullPath = Path.ChangeExtension(Path.Combine(basePath, fileName), extension);

            Logger.Info("Writing generated template contents to file at '{0}'.", fullPath);
            _fileWriter.WriteAllText(fullPath, context.TemplateContents);
        }
    }
}
