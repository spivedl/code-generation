using CodeGeneration.Extensions;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Services.Generation.Controller;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.Repository;
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
        private readonly IControllerGeneratorService _controllerGeneratorService;
        private readonly IRepositoryGeneratorService _repositoryGeneratorService;

        public BootService(ApplicationOptions applicationOptions, IModelGeneratorService modelGeneratorService, IViewGeneratorService viewGeneratorService, ISqlGeneratorService sqlGeneratorService, IControllerGeneratorService controllerGeneratorService, IRepositoryGeneratorService repositoryGeneratorService)
        {
            _applicationOptions = applicationOptions;
            _modelGeneratorService = modelGeneratorService;
            _viewGeneratorService = viewGeneratorService;
            _sqlGeneratorService = sqlGeneratorService;
            _controllerGeneratorService = controllerGeneratorService;
            _repositoryGeneratorService = repositoryGeneratorService;
        }

        public void Run()
        {
            Logger.Info("Boot Service says hello!");

            var embeddedResourceName = "CodeGeneration.Templates.ModelGenerator.Model.cshtml";
            var razorEngineKey = embeddedResourceName.ToRazorEngineKey();
            var fileName = embeddedResourceName.ToFileName();
            var templateName = embeddedResourceName.ToTemplateName();
            var tnToRazorEngineKey = templateName.ToRazorEngineKey("Templates.ModelGenerator");
            var fnToRazorEngineKey = fileName.ToRazorEngineKey("Templates.ModelGenerator");

            if (_applicationOptions.GenerateModels)
            {
                _modelGeneratorService.Generate(new ModelGenerationContext {ApplicationOptions = _applicationOptions});
                var modelCache = _modelGeneratorService.GetCache();
                var addressModel = _modelGeneratorService.GetCachedResult("Address");
                var modelTemplate = _modelGeneratorService.GetTemplate("Templates.ModelGenerator.Model");
            }

            if (_applicationOptions.GenerateModels && _applicationOptions.GenerateSql)
            {
                _sqlGeneratorService.Generate(new SqlGenerationContext { ApplicationOptions = _applicationOptions });
                var sqlCache = _sqlGeneratorService.GetCache();
                var addressInsertSql = _sqlGeneratorService.GetCachedResult("Address", "Insert");
                var insertTemplate = _sqlGeneratorService.GetTemplate("Insert", "Templates.SqlGenerator");
                var updateTemplate = _sqlGeneratorService.GetTemplate("Update", "Templates.SqlGenerator");
            }

            if (_applicationOptions.GenerateModels && _applicationOptions.GenerateViews)
            {
                _viewGeneratorService.Generate(new ViewGenerationContext { ApplicationOptions = _applicationOptions });
                var viewCache = _viewGeneratorService.GetCache();
                var addressCreateView = _viewGeneratorService.GetCachedResult("Address", "Create");
                var createTemplate = _viewGeneratorService.GetTemplate("Create", "Templates.ViewGenerator");
                var editTemplate = _viewGeneratorService.GetTemplate("Edit", "Templates.ViewGenerator");
            }

            if (_applicationOptions.GenerateModels && _applicationOptions.GenerateRepositories)
            {
                _repositoryGeneratorService.Generate(new RepositoryGenerationContext { ApplicationOptions = _applicationOptions });
                var repositoryCache = _repositoryGeneratorService.GetCache();
                var addressRepository = _repositoryGeneratorService.GetCachedResult("Address", "Repository");
                var repositoryTemplate = _repositoryGeneratorService.GetTemplate("Repository", "Templates.RepositoryGenerator");
            }

            if (_applicationOptions.GenerateModels && _applicationOptions.GenerateControllers)
            {
                _controllerGeneratorService.Generate(new ControllerGenerationContext { ApplicationOptions = _applicationOptions });
                var controllerCache = _controllerGeneratorService.GetCache();
                var addressController = _controllerGeneratorService.GetCachedResult("Address", "Controller");
                var controllerTemplate = _controllerGeneratorService.GetTemplate("Controller", "Templates.ControllerGenerator");
            }
        }
    }
}
