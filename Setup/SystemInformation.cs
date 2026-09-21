using System.Management;
using WindowsSetupTool.Helpers;

namespace WindowsSetupTool.Setup;

public static class SystemInformation
{
    public static string GetComputerName()
    {
        Logger.Info("Computername has been fetched");
        return Environment.MachineName;
    }

    public static string GetUserName()
    {
        Logger.Info("Current User has been fetched");
        return Environment.UserName;
    }

    public static string GetWindowsVersion()
    {
        Logger.Info("OS-Version has been fetched");
        return Environment.OSVersion.VersionString;
    }

    public static string GetProcessor()
    {
        using ManagementObjectSearcher searcher =
            new("SELECT Name FROM Win32_Processor");

        foreach (ManagementObject obj in searcher.Get())
        {
            return obj["Name"]?.ToString() ?? "Unbekannt";
        }

        return "Unbekannt";
    }
}