//---imports---
using System.Diagnostics;
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
                var psi = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-Command \"Rename-Computer -NewName '{hostname}' -Force\"",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(psi);
                Logger.Info($"Hostname set to: {hostname}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error while setting up Hostname: {ex}");
            }
        }
    }
}
