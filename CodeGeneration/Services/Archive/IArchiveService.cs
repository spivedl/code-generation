namespace CodeGeneration.Services.Archive
{
    public interface IArchiveService
    {
        bool Extract(string inputPath, string outputPath);
    }
}