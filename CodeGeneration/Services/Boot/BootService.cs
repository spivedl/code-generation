using System.Diagnostics;
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

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            _modelGeneratorService.Generate(new ModelGenerationContext { ApplicationOptions = _applicationOptions });
            _repositoryGeneratorService.Generate(new RepositoryGenerationContext { ApplicationOptions = _applicationOptions });
            _controllerGeneratorService.Generate(new ControllerGenerationContext { ApplicationOptions = _applicationOptions });
            _viewGeneratorService.Generate(new ViewGenerationContext { ApplicationOptions = _applicationOptions });
            _sqlGeneratorService.Generate(new SqlGenerationContext { ApplicationOptions = _applicationOptions });

            stopwatch.Stop();

            Logger.Info("Boot Service says goodbye! {0:g}", stopwatch.Elapsed);
        }
    }
}
