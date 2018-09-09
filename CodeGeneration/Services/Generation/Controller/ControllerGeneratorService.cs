using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Models.Template;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Template;
using NLog;

namespace CodeGeneration.Services.Generation.Controller
{
    public class ControllerGeneratorService : BaseGeneratorService, IControllerGeneratorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public ControllerGeneratorService(
            ITableMetadataService tableMetadataService,
            ITemplateService templateService,
            IFileWriter fileWriter) : base(tableMetadataService, templateService, fileWriter)
        {

        }

        protected override bool IsGeneratorEnabled(GenerationContext context)
        {
            if (context.ApplicationOptions.GenerateControllers) return true;

            Logger.Info("Controller generation is disabled. Change the 'GenerateControllers' option in the 'appsettings.json' file to enable.");
            return false;
        }

        protected override GenerationOptions GetGeneratorOptions(ApplicationOptions options)
        {
            return options.ControllerGeneration;
        }

        protected override TemplateModel GetTemplateModel(TemplateModelContext context)
        {
            return new ControllerTemplateModel(context);
        }
    }
}