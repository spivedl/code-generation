using System.Diagnostics;

namespace CodeGeneration.Services.Process
{
    public interface IProcessExecutorService
    {
        System.Diagnostics.Process Execute(string path, string arguments = null);
        System.Diagnostics.Process Execute(ProcessStartInfo processStartInfo);
    }
}