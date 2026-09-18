using Microsoft.Win32;
using WindowsSetupTool.Helpers;

namespace WindowsSetupTool.Setup;

public static class ExplorerSettings
{
    public static void EnableClassicContextMenu()
    {
        using var key = Registry.CurrentUser.CreateSubKey(
            @"Software\Classes\CLSID\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\InprocServer32");
        key.SetValue("", "", RegistryValueKind.String);

        RestartExplorer();
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

        Logger.Log("Dateiendungen aktiviert.");
    }

    private static void RestartExplorer()
    {
        foreach (var process in System.Diagnostics.Process.GetProcessesByName("explorer"))
            process.Kill();
        System.Diagnostics.Process.Start("explorer.exe");
    }
}