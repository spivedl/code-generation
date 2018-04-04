using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using NLog;

namespace CodeGeneration.Services.Archive
{
    public class ZipArchiveService : IArchiveService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool Extract(string inputPath, string outputPath)
        {
            try
            {
                Logger.Info($"Begin extract of file '{inputPath}' to '{outputPath}'.");

                if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

                using (var zip = ZipFile.OpenRead(inputPath))
                {
                    var size = 0.0;
                    var compressedSize = 0.0;
                    var stopwatchAll = new Stopwatch();
                    var stopwatchFile = new Stopwatch();

                    stopwatchAll.Start();

                    var maxLengthFileName = zip.Entries.Select(e => e.FullName).Max(x => x.Length);
                    var padLength = maxLengthFileName + 5;

                    Logger.Info("{0}{1}{2}", "FILE_NAME".PadRight(padLength), "SIZE(KB)".PadRight(padLength), "SECONDS");
                    Logger.Info("{0}", new string('-', padLength * 3));

                    foreach (var entry in zip.Entries)
                    {
                        size += entry.Length;
                        compressedSize += entry.CompressedLength;

                        stopwatchFile.Start();

                        var path = Path.Combine(outputPath, entry.FullName);
                        if (path.EndsWith("/"))
                        {
                            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        }
                        else
                        {
                            entry.ExtractToFile(path, overwrite: true);
                        }

                        stopwatchFile.Stop();

                        Logger.Info("{0}{1}{2}",
                            entry.FullName.PadRight(padLength),
                            (entry.Length / 1000.0).ToString("00.###").PadRight(padLength),
                            stopwatchFile.Elapsed.TotalSeconds);

                        stopwatchFile.Reset();
                    }

                    stopwatchAll.Stop();

                    Logger.Info("{0}", new string('-', padLength * 3));
                    Logger.Info("Extract completed successfully!");
                    Logger.Info($"File(s): {zip.Entries.Count}");
                    Logger.Info($"Compressed Size: {compressedSize / 1000000} MB");
                    Logger.Info($"Total Size: {size / 1000000} MB");
                    Logger.Info($"Total Time: {stopwatchAll.Elapsed.TotalSeconds} seconds");
                    Logger.Info("{0}", new string('-', padLength * 3));

                    Logger.Info($"Finished extract of file '{inputPath}' to '{outputPath}'.");

                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Exception extracting archive at '{inputPath}'.");
                return false;
            }
        }
    }
}