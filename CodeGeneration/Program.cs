using System.IO;
using Autofac;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Modules;
using CodeGeneration.Services.Archive;
using CodeGeneration.Services.Boot;
using CodeGeneration.Services.Cache;
using CodeGeneration.Services.Compiler;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Generation;
using CodeGeneration.Services.Generation.Controller;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.Repository;
using CodeGeneration.Services.Generation.Sql;
using CodeGeneration.Services.Generation.StaticFile;
using CodeGeneration.Services.Generation.View;
using CodeGeneration.Services.Process;
using CodeGeneration.Services.Template;
using CodeGeneration.Services.Template.Liquid;
using CodeGeneration.Services.Template.Razor;
using Microsoft.Extensions.Configuration;
using NLog;

namespace CodeGeneration
{
    public class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var container = new ContainerBuilder();
            var configuration = BuildConfiguration(args);
            var appSettings = BindAppSettings(configuration);

            // register configuration and ApplicationOptions with container
            container.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();
            container.RegisterInstance(appSettings).As<ApplicationOptions>().SingleInstance();

            // register caching module
            container.RegisterModule<CachingModule>();

            // register services with container
            container.RegisterType<ZipArchiveService>().As<IArchiveService>().SingleInstance();
            container.RegisterType<BootService>().As<IBootService>().SingleInstance();
            container.RegisterType<MemoryCacheService>().As<ICacheService>();
            container.RegisterType<CSharpInMemoryCompiler>().As<ICompilerService>().SingleInstance();
            container.RegisterType<DataService>().As<IDataService>().SingleInstance();
            container.RegisterType<TableMetadataService>().As<ITableMetadataService>().SingleInstance();
            container.RegisterType<FileReader>().As<IFileReader>().SingleInstance();
            container.RegisterType<FileWriter>().As<IFileWriter>().SingleInstance();
            container.RegisterType<ProcessExecutorService>().As<IProcessExecutorService>().SingleInstance();
            container.RegisterType<SqlCommandExecutorService>().As<ISqlCommandExecutorService>().SingleInstance();
            container.RegisterType<RazorTemplateService>().As<IRazorTemplateService>().SingleInstance();
            container.RegisterType<LiquidTemplateService>().As<ITemplateService>().SingleInstance();
            container.RegisterType<ControllerGeneratorService>().As<IControllerGeneratorService>().SingleInstance();
            container.RegisterType<ModelGeneratorService>().As<IModelGeneratorService>().SingleInstance();
            //container.RegisterType<ModelGeneratorService>().As<IModelGeneratorService>().SingleInstance();
            container.RegisterType<RepositoryGeneratorService>().As<IRepositoryGeneratorService>().SingleInstance();
            container.RegisterType<SqlGeneratorService>().As<ISqlGeneratorService>().SingleInstance();
            container.RegisterType<StaticFileGeneratorService>().As<IStaticFileGeneratorService>().SingleInstance();
            container.RegisterType<ViewGeneratorService>().As<IViewGeneratorService>().SingleInstance();

            // resolve an instance of the boot service and run it to kick off generation
            using (var scope = container.Build().BeginLifetimeScope())
            {
                var bootService = scope.Resolve<IBootService>();
                bootService.Run();
            }
        }

        private static IConfiguration BuildConfiguration(string[] args)
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddCommandLine(args)
                .Build();
        }

        private static ApplicationOptions BindAppSettings(IConfiguration configuration)
        {
            var appSettings = new ApplicationOptions();

            configuration.GetSection("Application").Bind(appSettings);

            return appSettings;
        }
    }
}
