using System.Diagnostics;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.Context;
using CodeGeneration.Services.Generation;
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

            var generationContext = new GenerationContext { ApplicationOptions = _applicationOptions };

            GenerateControllers(generationContext);

            var modelStats = GenerateModels(generationContext);

            GenerateRepositories(generationContext);

            GenerateSql(generationContext);

            GenerateStaticFiles(generationContext);

            GenerateViews(generationContext);

            stopwatch.Stop();

            Logger.Info("Boot Service says goodbye! {0:g}", stopwatch.Elapsed);
        }

        private void GenerateControllers(GenerationContext context)
        {
            if (!_applicationOptions.GenerateControllers)
            {
                Logger.Info("Controller generation is disabled. Change the 'GenerateControllers' option in the 'appsettings.json' file to enable.");
                return;
            }

            _controllerGeneratorService
                .GenerateOutput(context)
                .Report(Logger, "Controller Generation completed with the following statistics:");
        }

        private GenerationStatistics GenerateModels(GenerationContext context)
        {
            if (!_applicationOptions.GenerateModels)
            {
                Logger.Info("Model generation is disabled. Change the 'GenerateModels' option in the 'appsettings.json' file to enable.");
                return new GenerationStatistics { GenerationEnabled = false };
            }

            return _modelGeneratorService
                .GenerateOutput(context)
                .Report(Logger, "Model Generation completed with the following statistics:");
        }

        private void GenerateRepositories(GenerationContext context)
        {
            if (!_applicationOptions.GenerateRepositories)
            {
                Logger.Info("Repository generation is disabled. Change the 'GenerateRepositories' option in the 'appsettings.json' file to enable.");
                return;
            }

            _repositoryGeneratorService.Generate(context);
        }

        private void GenerateSql(GenerationContext context)
        {
            if (!_applicationOptions.GenerateSql)
            {
                Logger.Info("SQL generation is disabled. Change the 'GenerateSql' option in the 'appsettings.json' file to enable.");
                return;
            }

            _sqlGeneratorService.Generate(context);
        }

        private void GenerateStaticFiles(GenerationContext context)
        {
            if (!_applicationOptions.GenerateStaticFiles)
            {
                Logger.Info("Static File generation is disabled. Change the 'GenerateStaticFiles' option in the 'appsettings.json' file to enable.");
                return;
            }

            _staticFileGeneratorService.Generate(context);
        }

        private void GenerateViews(GenerationContext context)
        {
            if (!_applicationOptions.GenerateViews)
            {
                Logger.Info("View generation is disabled. Change the 'GenerateViews' option in the 'appsettings.json' file to enable.");
                return;
            }

            _viewGeneratorService.Generate(context);
        }
    }
}
