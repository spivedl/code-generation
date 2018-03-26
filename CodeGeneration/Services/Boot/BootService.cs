using CodeGeneration.Models.Configuration;
using CodeGeneration.Models.GenerationContext;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.View;
using NLog;

namespace CodeGeneration.Services.Boot
{
    public class BootService : IBootService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly AppSettings _appSettings;
        private readonly ITableMetadataService _tableMetadataService;
        private readonly IViewGeneratorService _viewGeneratorService;
        private readonly IModelGeneratorService _modelGeneratorService;

        public BootService(AppSettings appSettings, ITableMetadataService tableMetadataService, IViewGeneratorService viewGeneratorService, IModelGeneratorService modelGeneratorService)
        {
            _appSettings = appSettings;
            _tableMetadataService = tableMetadataService;
            _viewGeneratorService = viewGeneratorService;
            _modelGeneratorService = modelGeneratorService;
        }

        public void Run()
        {
            Logger.Info("Boot Service says hello!");
            
            //_viewGeneratorService.Generate(new ViewGenerationContext{ AppSettings = _appSettings});
            _tableMetadataService.GetTableMetadata("keeptime", _appSettings.SqlGeneration.SourceDatabase, _appSettings.SqlGeneration.SourceSchema);
            _modelGeneratorService.Generate(new ModelGenerationContext{AppSettings = _appSettings});
            var test = _modelGeneratorService.Get("address", "model");
        }
    }
}
