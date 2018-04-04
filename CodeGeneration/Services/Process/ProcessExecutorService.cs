using System;
using System.Diagnostics;
using System.Threading;
using NLog;

namespace CodeGeneration.Services.Process
{
    public class ProcessExecutorService : IProcessExecutorService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const int ProcessTimeout = 15;

        public System.Diagnostics.Process Execute(string path, string arguments = null)
        {
            var process = CreateProcess(path, arguments);
            return Execute(process);
        }

        public System.Diagnostics.Process Execute(ProcessStartInfo processStartInfo)
        {
            var process = CreateProcess(processStartInfo);
            return Execute(process);
        }

        private static System.Diagnostics.Process Execute(System.Diagnostics.Process process)
        {
            var processIdentifier = $"'{process.StartInfo.FileName} {process.StartInfo.Arguments}'";

            try
            {
                if (process.Start())
                {
                    Logger.Info($"Starting process {processIdentifier}.");
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    while (!process.HasExited)
                    {
                        if (DateTime.Now.Subtract(process.StartTime) <= new TimeSpan(hours: 0, minutes: ProcessTimeout, seconds: 0))
                        {
                            if (process.Responding)
                            {
                                Thread.Sleep(TimeSpan.FromSeconds(5));
                            }
                            else
                            {
                                Logger.Error($"Process {processIdentifier} not responding!");
                            }
                        }
                        else
                        {
                            throw new Exception($"Process {processIdentifier} has timed out before exiting!");
                        }
                    }
                }
                else
                {
                    throw new Exception($"Process {processIdentifier} failed to start.");
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Error in process {processIdentifier}.");
            }

            return process;
        }

        private static System.Diagnostics.Process CreateProcess(string path, string arguments)
        {
            return CreateProcess(new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            });
        }

        private static System.Diagnostics.Process CreateProcess(ProcessStartInfo processStartInfo)
        {
            var process = new System.Diagnostics.Process { StartInfo = processStartInfo };

            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;
            process.Exited += Process_Exited;

            return process;
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data)) return;

            Logger.Info(e.Data);
        }

        private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.Data)) return;

            Logger.Error(e.Data);
        }

        private static void Process_Exited(object sender, EventArgs e)
        {
            var process = (System.Diagnostics.Process)sender;

            Logger.Info($"Process '{process.StartInfo.FileName} {process.StartInfo.Arguments}' has exited.");
            Logger.Info($"StartTime: {process.StartTime}.");
            Logger.Info($"ExitTime: {process.ExitTime}.");
            Logger.Info($"ExitCode: {process.ExitCode}.");

            if (process.ExitCode != 0)
                Logger.Error($"Process exited with code other than zero. Exit code was '{process.ExitCode}'.");
        }
    }
}
