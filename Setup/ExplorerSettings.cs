using Microsoft.Win32;
using System.Diagnostics;
using System.Threading;
using WindowsSetupTool.Helpers;

namespace WindowsSetupTool.Setup;

public static class ExplorerSettings
{
    public static void EnableClassicContextMenu()
    {
        using var key = Registry.CurrentUser.CreateSubKey(
            @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
        key.SetValue("", "", RegistryValueKind.String);

        RestartExplorerSilently();

        Logger.Log("activated classic contextmenu");
    }

    public static void ShowFileExtensions()
    {
        using RegistryKey? key =
            Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced");

        key?.SetValue(
            "HideFileExt",
            0,
            RegistryValueKind.DWord);

        Logger.Log("show file extensions enabled");
    }

    public static void RestartExplorerSilently()
    {
        var explorerProcesses = Process.GetProcessesByName("explorer");
        foreach (var process in explorerProcesses)
            process.Kill();

        // Windows startet den Shell-Prozess selbstständig neu.
        // Kurz abwarten und prüfen, ob er wieder läuft.
        for (int i = 0; i < 20; i++) // max. ~10 Sekunden warten
        {
            Thread.Sleep(500);
            if (Process.GetProcessesByName("explorer").Length > 0)
                return; // Shell ist wieder da, fertig
        }

        // Falls Windows es ausnahmsweise nicht selbst getan hat: manuell nachhelfen
        Process.Start(new ProcessStartInfo("explorer.exe") { UseShellExecute = true });
    }
}