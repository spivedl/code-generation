using CodeGeneration.Models.Configuration;

namespace CodeGeneration.Services.Process
{
    public interface ISqlCommandExecutorService : IProcessExecutorService
    {
        System.Diagnostics.Process ExecuteSqlCommand(string inputFile, SqlCommandOptions options);
    }
}