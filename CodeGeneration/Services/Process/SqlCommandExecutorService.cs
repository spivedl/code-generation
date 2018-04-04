using System;
using System.Diagnostics;
using System.IO;
using CodeGeneration.Models.Configuration;
using CodeGeneration.Services.Archive;
using NLog;

namespace CodeGeneration.Services.Process
{
    public class SqlCommandExecutorService : ISqlCommandExecutorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IArchiveService _archiveService;
        private readonly IProcessExecutorService _processExecutorService;
        private readonly string _sqlCommandPath;

        public SqlCommandExecutorService(ApplicationOptions applicationOptions, IArchiveService archiveService, IProcessExecutorService processExecutorService)
        {
            _archiveService = archiveService;
            _processExecutorService = processExecutorService;

            Logger.Info("Initializing SqlCommandExecutor service.");

            _sqlCommandPath = ExtractSqlCommandResources(applicationOptions.SqlGeneration.SqlCommand);

            Logger.Info("Finished initializing SqlCommandExecutor service.");
        }

        private string ExtractSqlCommandResources(SqlCommandOptions options)
        {
            if (!options.ExecuteSql) return string.Empty;

            try
            {
                var zipName = $"{options.EmbeddedExecutable}.zip";
                var exeName = $"{options.EmbeddedExecutable}.exe";

                var tempPath = Path.GetTempPath();
                var archivePath = Path.Combine(tempPath, zipName);

                Logger.Info($"Writing embedded SQLCMD archive to '{tempPath}'.");
                System.IO.File.WriteAllBytes(archivePath, Properties.Resources.SQLCMD_ARCHIVE);

                _archiveService.Extract(archivePath, tempPath);

                return Path.Combine(tempPath, options.EmbeddedExecutable, exeName);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error attempting to extract embedded SQLCMD resources.");
                throw;
            }
        }

        public System.Diagnostics.Process ExecuteSqlCommand(string inputFile, SqlCommandOptions options)
        {
            if (!options.ExecuteSql) return null;

            var arguments = BuildArguments(inputFile, options);
            return Execute(_sqlCommandPath, arguments);
        }

        private static string BuildArguments(string inputFile, SqlCommandOptions options)
        {
            var args = new ArgumentBuilder("-S", options.TargetServer, "-d", options.TargetDatabase, "-i", $"\"{inputFile}\"");

            if (options.UseTrustedConnection) args.Add("-E");
            else
            {
                args.Add("-U", options.User)
                    .Add("-P", options.Password);
            }

            if (options.EchoInput) args.Add("-e");

            args.Add("-i", $"\"{inputFile}\"");

            return args.Build();
        }

        public System.Diagnostics.Process Execute(string path, string arguments = null)
        {
            return _processExecutorService.Execute(path, arguments);
        }

        public System.Diagnostics.Process Execute(ProcessStartInfo processStartInfo)
        {
            return _processExecutorService.Execute(processStartInfo);
        }
    }
}