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
        public static void SetHostname(string hostname)
        {
            if (hostname == null || hostname.Length <= 0)
            {
                Logger.Error("No hostname was specified - Skipping task!");
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
                }
                else
                {
                    Logger.Info($"Hostname set to: {hostname}");
                }            
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while trying to set up Hostname: {ex}");
            }
        }
    }
}
