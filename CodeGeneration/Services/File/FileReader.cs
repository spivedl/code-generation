using System;
using NLog;

namespace CodeGeneration.Services.File
{
    public class FileReader : IFileReader
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string ReadAllText(string path)
        {
            try
            {
                var contents = System.IO.File.ReadAllText(path);
                LogContents(path, contents);

                return contents;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error reading file at '{path}'.");
                throw;
            }
        }

        public string[] ReadAllLines(string path)
        {
            try
            {
                var contents = System.IO.File.ReadAllLines(path);
                LogContents(path, string.Join(Environment.NewLine, contents));

                return contents;
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error reading file at '{path}'.");
                throw;
            }
        }

        private static void LogContents(string path, string contents)
        {
            Logger.Info($"Read the following contents from file at '{path}'.");
            Logger.Info(contents);
        }
    }
}