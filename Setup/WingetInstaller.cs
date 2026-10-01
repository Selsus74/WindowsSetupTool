//---imports---
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using WindowsSetupTool.Helpers;
using WindowsSetupTool.Models;
//---namespace---
namespace WindowsSetupTool.Setup
{
    //---result---
    internal record InstallResult(bool Success, int ExitCode, string Output);

    //---class init---
    internal class WingetInstaller
    {
        //---winget-Exit-Code 0x8A150061 = paket already installed---
        private const int AlreadyInstalled = unchecked((int)0x8A150061);

        //---task for installing software---
        public static async Task InstallAsync(Software software, SetupProgressHelper progress)
        {
            //---setting process and its arguments---
            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                Arguments = $"install --exact --id {software.PaketId} --source winget " +
                            "--silent --accept-package-agreements --accept-source-agreements",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };
            //---read all process output---
            var output = new StringBuilder();
            using var process = new Process { StartInfo = startInfo };
            process.OutputDataReceived += (_, e) => { if (e.Data is not null) output.AppendLine(e.Data); };
            process.ErrorDataReceived += (_, e) => { if (e.Data is not null) output.AppendLine(e.Data); };

            try
            {
                //---logging process start---
                Logger.Info($"Trying to install {software.Name} now...");
                //---starts process and waits for it to finish---
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                await process.WaitForExitAsync();
            }
            catch (Win32Exception)
            {
                //---if winget is not installed---
                Logger.Error($"Error while trying to install {software.Name}: Winget not available!");
                progress.ReportTaskFailed();
                return;
            }
            catch (OperationCanceledException)
            {
                //---log this also---
                Logger.Error($"Error while trying to install {software.Name}: Process got killed!");
                progress.ReportTaskFailed();
                if (!process.HasExited) process.Kill(entireProcessTree: true);
                throw;
            }

            if (process.ExitCode == 0 || process.ExitCode == AlreadyInstalled)
            {
                //---log it!---
                Logger.Info
                (
                    $"{software.Name} successfully installed. Exit-Code: {process.ExitCode}" + "\n" + $"{output}"
                );
                //---return result---
                progress.ReportTaskCompleted();
                return;
            }
            else
            {
                //---log it!---
                Logger.Error
                (
                    $"Error while trying to install {software.Name} - Exit-Code: {process.ExitCode} \n" +
                    $"StackTrace: \n" +
                    $"{output}"
                );
                progress.ReportTaskFailed();
                return;
            }
        }
    }
}