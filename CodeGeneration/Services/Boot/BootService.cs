using System.Diagnostics;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Services.Generation.Controller;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.Repository;
using CodeGeneration.Services.Generation.Sql;
using CodeGeneration.Services.Generation.StaticFile;
using CodeGeneration.Services.Generation.View;
using NLog;

namespace CodeGeneration.Services.Boot
{
    public class BootService : IBootService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly ApplicationOptions _applicationOptions;
        private readonly IControllerGeneratorService _controllerGeneratorService;
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly IRepositoryGeneratorService _repositoryGeneratorService;
        private readonly ISqlGeneratorService _sqlGeneratorService;
        private readonly IStaticFileGeneratorService _staticFileGeneratorService;
        private readonly IViewGeneratorService _viewGeneratorService;

        public BootService(ApplicationOptions applicationOptions, IControllerGeneratorService controllerGeneratorService, IModelGeneratorService modelGeneratorService, IRepositoryGeneratorService repositoryGeneratorService, ISqlGeneratorService sqlGeneratorService, IStaticFileGeneratorService staticFileGeneratorService, IViewGeneratorService viewGeneratorService)
        {
            _applicationOptions = applicationOptions;
            _controllerGeneratorService = controllerGeneratorService;
            _modelGeneratorService = modelGeneratorService;
            _repositoryGeneratorService = repositoryGeneratorService;
            _sqlGeneratorService = sqlGeneratorService;
            _staticFileGeneratorService = staticFileGeneratorService;
            _viewGeneratorService = viewGeneratorService;
        }

        public void Run()
        {
            Logger.Info("Boot Service says hello!");

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var defaultGenerationContext = new GenerationContext { ApplicationOptions = _applicationOptions };

            _controllerGeneratorService.Generate(defaultGenerationContext);

            _modelGeneratorService.Generate(defaultGenerationContext);

            _repositoryGeneratorService.Generate(defaultGenerationContext);

            _sqlGeneratorService.Generate(defaultGenerationContext);

            _staticFileGeneratorService.Generate(defaultGenerationContext);

            _viewGeneratorService.Generate(defaultGenerationContext);

            stopwatch.Stop();

            Logger.Info("Boot Service says goodbye! {0:g}", stopwatch.Elapsed);
        }
    }
}
