using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace CodeGeneration.Services.File
{
    public class FileWriter : IFileWriter
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public void WriteAllLines(string path, string[] contents)
        {
            try
            {
                CheckDirectory(path);
                LogContents(path, string.Join(Environment.NewLine, contents));

                System.IO.File.WriteAllLines(path, contents);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error writing file at '{path}'.");
                throw;
            }
        }

        public void WriteAllText(string path, string contents)
        {
            try
            {
                CheckDirectory(path);
                LogContents(path, contents);

                System.IO.File.WriteAllText(path, contents);
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error writing file at '{path}'.");
                throw;
            }
        }

        public void WriteLines(string path, IEnumerable<string> contents, bool append)
        {
            try
            {
                CheckDirectory(path);

                var lines = contents.ToList();
                LogContents(path, string.Join(Environment.NewLine, lines));

                using (var file = new StreamWriter(path, append))
                {
                    foreach (var line in lines)
                    {
                        file.WriteLine(line);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error writing file at '{path}'.");
                throw;
            }
        }

        private static void CheckDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (Directory.Exists(directory)) return;

            if (directory == null) throw new Exception("Path cannot be null or empty!");

            Logger.Info($"Directory '{directory}' does not exist. Creating directory before attempting to write file...");
            Directory.CreateDirectory(directory);
        }

        private static void LogContents(string path, string contents)
        {
            Logger.Info($"Writing the following contents to file at '{path}'.");
            Logger.Info(contents);
        }
    }
}