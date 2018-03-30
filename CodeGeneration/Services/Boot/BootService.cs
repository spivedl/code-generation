using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.Sql;
using CodeGeneration.Services.Generation.View;
using NLog;

namespace CodeGeneration.Services.Boot
{
    public class BootService : IBootService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ApplicationOptions _applicationOptions;
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly IViewGeneratorService _viewGeneratorService;
        private readonly ISqlGeneratorService _sqlGeneratorService;

        public BootService(ApplicationOptions applicationOptions, IModelGeneratorService modelGeneratorService, IViewGeneratorService viewGeneratorService, ISqlGeneratorService sqlGeneratorService)
        {
            _applicationOptions = applicationOptions;
            _modelGeneratorService = modelGeneratorService;
            _viewGeneratorService = viewGeneratorService;
            _sqlGeneratorService = sqlGeneratorService;
        }

        public void Run()
        {
            Logger.Info("Boot Service says hello!");

            /*var embeddedResourceName = "CodeGeneration.Templates.ModelGenerator.Model.cshtml";
            var razorEngineKey = embeddedResourceName.ToRazorEngineKey();
            var fileName = embeddedResourceName.ToFileName();
            var templateName = embeddedResourceName.ToTemplateName();
            var tnToRazorEngineKey = templateName.ToRazorEngineKey("Templates.ModelGenerator");
            var fnToRazorEngineKey = fileName.ToRazorEngineKey("Templates.ModelGenerator");*/

            _modelGeneratorService.Generate(new ModelGenerationContext{ ApplicationOptions = _applicationOptions });
            var modelCache = _modelGeneratorService.GetCache();
            var addressModel = _modelGeneratorService.GetCachedResult("Address");
            var modelTemplate = _modelGeneratorService.GetTemplate("Templates.ModelGenerator.Model");

            /*_viewGeneratorService.Generate(new ViewGenerationContext { ApplicationOptions = _applicationOptions });
            var viewCache = _viewGeneratorService.GetCache();
            var addressCreateView = _viewGeneratorService.GetCachedResult("Address", "Create");
            var createTemplate = _viewGeneratorService.GetTemplate("Create", "Templates.ViewGenerator");
            var editTemplate = _viewGeneratorService.GetTemplate("Edit", "Templates.ViewGenerator");*/

            _sqlGeneratorService.Generate(new SqlGenerationContext { ApplicationOptions = _applicationOptions });
            var sqlCache = _sqlGeneratorService.GetCache();
            var addressInsertSql = _sqlGeneratorService.GetCachedResult("Address", "Insert");
            var insertTemplate = _sqlGeneratorService.GetTemplate("Insert", "Templates.SqlGenerator");
            var updateTemplate = _sqlGeneratorService.GetTemplate("Update", "Templates.SqlGenerator");
        }
    }
}
