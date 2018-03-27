using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.Generation.Model;
using NLog;

namespace CodeGeneration.Services.Boot
{
    public class BootService : IBootService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AppSettings _appSettings;
        private readonly ITableMetadataService _tableMetadataService;
        private readonly IModelGeneratorService _modelGeneratorService;

        public BootService(AppSettings appSettings, ITableMetadataService tableMetadataService, IModelGeneratorService modelGeneratorService)
        {
            _appSettings = appSettings;
            _tableMetadataService = tableMetadataService;
            _modelGeneratorService = modelGeneratorService;
        }

        public void Run()
        {
            Logger.Info("Boot Service says hello!");
            
            //_viewGeneratorService.Generate(new ViewGenerationContext{ AppSettings = _appSettings});
            _tableMetadataService.GetTableMetadata("keeptime", _appSettings.SqlGeneration.SourceDatabase, _appSettings.SqlGeneration.SourceSchema);
            _modelGeneratorService.Generate(new ModelGenerationContext{AppSettings = _appSettings});

            var cachedResult = _modelGeneratorService.GetCachedResult("address", "model");
            var template = _modelGeneratorService.GetTemplate("Templates.ModelGenerator.Model");
        }
    }
}
