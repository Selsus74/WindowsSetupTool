//---imports---
using System.Management.Automation;
using WindowsSetupTool.Helpers;
//---namespace---
namespace WindowsSetupTool.Setup
{
    //---class init---
    internal class SystemSettings
    {
        //---method for setting hostname---
        public static Task SetHostname(string hostname, SetupProgressHelper progress)
        {
            return Task.Run(() =>
            {
                if (hostname == null || hostname.Length <= 0)
                {
                    Logger.Error("No hostname was specified - Skipping task!");
                    progress.ReportTaskFailed();
                    return;
                }

                try
                {
                    using var ps = PowerShell.Create();
                    ps.AddScript($"Rename-Computer -NewName '{hostname}' -Force -ErrorAction Stop");
                    var results = ps.Invoke();

                    if (ps.HadErrors)
                    {
                        foreach (var error in ps.Streams.Error)
                        {
                            Logger.Error($"Error while trying to set up {hostname} as Hostname: {error.Exception.Message}");
                        }
                        progress.ReportTaskFailed();
                    }
                    else
                    {
                        Logger.Info($"Hostname set to: {hostname}");
                        progress.ReportTaskCompleted();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Error while trying to set up Hostname: {ex}");
                    progress.ReportTaskFailed();
                }
            });
        }
    }
}
