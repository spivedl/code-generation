using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Template;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Template;
using NLog;

namespace CodeGeneration.Services.Generation.Model
{
    public class ModelGeneratorService : BaseGeneratorService, IModelGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ModelGeneratorService(
            ITableMetadataService tableMetadataService,
            ITemplateService templateService,
            IFileWriter fileWriter) : base(tableMetadataService, templateService, fileWriter)
        {
        }

        protected override bool IsGeneratorEnabled(GenerationContext context)
        {
            if (context.ApplicationOptions.GenerateModels) return true;

            Logger.Info("Model generation is disabled. Change the 'GenerateModels' option in the 'appsettings.json' file to enable.");
            return false;
        }

        protected override GenerationOptions GetGeneratorOptions(ApplicationOptions options)
        {
            return options.ModelGeneration;
        }

        protected override TemplateModel GetTemplateModel(TemplateModelContext context)
        {
            return new TableMetadataTemplateModel(context.ConnectionKey, context.TargetNamespace, context.TableMetadata);
        }
    }
}
