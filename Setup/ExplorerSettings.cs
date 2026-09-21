using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using WindowsSetupTool.Helpers;

namespace WindowsSetupTool.Setup;

public static class ExplorerSettings
{
    //---enables the win10 style context menu---
    public static void EnableClassicContextMenu()
    {
        using var key = Registry.CurrentUser.CreateSubKey(
            @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
        key.SetValue("", "", RegistryValueKind.String);

        Logger.Info("Enabled Classic Contextmenu");

        RestartExplorerSilently();
    }

    //---shows the file extensions in explorer---
    public static void ShowFileExtensions()
    {
        using RegistryKey? key =
            Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");

        key?.SetValue(
            "HideFileExt",
            0,
            RegistryValueKind.DWord);

        Logger.Info("Enabled File Extensions");
    }

    //---method for restarting the explorer after pending changes---
    public static void RestartExplorerSilently()
    {
        var explorerProcesses = Process.GetProcessesByName("explorer");
        foreach (var process in explorerProcesses)
            process.Kill();
        Logger.Info("Explorer-Process has been stopped - waiting for restart...");

        //---windows automatically restarts the explorer after a moment
        //---wait and check if it runs again
        for (int i = 0; i < 20; i++) // waits for ~10sec
        {
            Thread.Sleep(500);
            if (Process.GetProcessesByName("explorer").Length > 0)
            {
                Logger.Info("Explorer has been auto restarted");
                return; // exits if process is automatically restarted within 10secs
            }
        }

        //---if not autostarted in time, gets manually started---
        Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
        Logger.Warning("Explorer has been manually restarted");
    }
}