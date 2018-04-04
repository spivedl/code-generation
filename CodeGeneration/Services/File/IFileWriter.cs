using System.Collections.Generic;

namespace CodeGeneration.Services.File
{
    public interface IFileWriter
    {
        void WriteAllLines(string path, string[] contents);
        void WriteAllText(string path, string contents);
        void WriteLines(string path, string contents, bool append = false);
        void WriteLines(string path, IEnumerable<string> contents, bool append = false);
    }
}