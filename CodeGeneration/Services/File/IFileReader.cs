namespace CodeGeneration.Services.File
{
    public interface IFileReader
    {
        string ReadAllText(string path);

        string[] ReadAllLines(string path);
    }
}