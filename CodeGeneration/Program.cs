using System.IO;
using Autofac;
using Autofac.Builder;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Services.Boot;
using CodeGeneration.Services.Compiler;
using CodeGeneration.Services.Data;
using CodeGeneration.Services.File;
using CodeGeneration.Services.Generation.Model;
using CodeGeneration.Services.Generation.View;
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

            // register services with container
            container.RegisterInstance(configuration).As<IConfiguration>().SingleInstance();
            container.RegisterInstance(appSettings).As<AppSettings>().SingleInstance();
            container.RegisterType<DataService>().As<IDataService>().SingleInstance();
            container.RegisterType<TableMetadataService>().As<ITableMetadataService>().SingleInstance();
            container.RegisterType<FileReader>().As<IFileReader>().SingleInstance();
            container.RegisterType<FileWriter>().As<IFileWriter>().SingleInstance();
            container.RegisterType<CSharpInMemoryCompiler>().As<ICompilerService>().SingleInstance();
            container.RegisterType<RazorTemplateService>().As<IRazorTemplateService>().SingleInstance();
            container.RegisterType<ViewGeneratorService>().As<IViewGeneratorService>().SingleInstance();
            container.RegisterType<ModelGeneratorService>().As<IModelGeneratorService>().SingleInstance();
            container.RegisterType<BootService>().As<IBootService>().SingleInstance();

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

        private static AppSettings BindAppSettings(IConfiguration configuration)
        {
            var appSettings = new AppSettings();

            configuration.GetSection("App").Bind(appSettings);

            return appSettings;
        }
    }
}
