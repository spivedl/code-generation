using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.View;
using NLog;

namespace CodeGeneration.Services.Boot
{
    public class BootService : IBootService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AppSettings _appSettings;
        private readonly IModelGeneratorService _modelGeneratorService;
        private readonly IViewGeneratorService _viewGeneratorService;

        public BootService(AppSettings appSettings, IModelGeneratorService modelGeneratorService, IViewGeneratorService viewGeneratorService)
        {
            _appSettings = appSettings;
            _modelGeneratorService = modelGeneratorService;
            _viewGeneratorService = viewGeneratorService;
        }

        public void Run()
        {
            Logger.Info("Boot Service says hello!");
            
            _modelGeneratorService.Generate(new ModelGenerationContext{ AppSettings = _appSettings });
            var cachedResult = _modelGeneratorService.GetCachedResult("address", "model");
            var template = _modelGeneratorService.GetTemplate("Templates.ModelGenerator.Model");

            _viewGeneratorService.Generate(new ViewGenerationContext { AppSettings = _appSettings });
            cachedResult = _viewGeneratorService.GetCachedResult("address", "model");
            template = _viewGeneratorService.GetTemplate("Templates.ModelGenerator.Model");
        }
    }
}
