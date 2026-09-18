using Microsoft.Win32;
using WindowsSetupTool.Helpers;

namespace WindowsSetupTool.Setup;

public static class ExplorerSettings
{
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
}